using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using Xunit;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Shared.Interfaces;

namespace BookingSystem.IntegrationTests.Checkout.Utils;

public class CheckoutTestFixture : IAsyncLifetime
{
    public ApiTestFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;
    public TestEmailService EmailService { get; private set; } = null!;
    public TestPaymentService PaymentService { get; private set; } = null!;
    public TestPdfService PdfService { get; private set; } = null!;
    public InMemoryUnitOfWork InMemoryUow { get; private set; } = null!;
    public AppDbContext DbContext { get; private set; } = null!;
    public Guid TestUserId { get; private set; }
    private IServiceScope _scope = null!;

    public CheckoutTestFixture()
    {
        // initialize HttpClient, DbContext, test data, etc.
    }
    public async Task InitializeAsync()
    {
        Factory = new ApiTestFactory();
        Factory.SetInMemoryDbName($"CheckoutTests_{Guid.NewGuid()}");

        EmailService = new TestEmailService();
        PaymentService = new TestPaymentService();
        PdfService = new TestPdfService();

        // Override external services with test implementations
        Factory.AddServiceConfiguration(services =>
        {
            // Remove external services
            RemoveService<IEmailService>(services);
            RemoveService<IPaymentService>(services);
            RemoveService<IPdfService>(services);
            RemoveService<IUnitOfWork>(services);

            services.AddSingleton<IEmailService>(EmailService);
            services.AddSingleton<IPaymentService>(PaymentService);
            services.AddSingleton<IPdfService>(PdfService);
            //     services.AddSingleton<IUnitOfWork>(InMemoryUow);
            services.AddScoped<IUnitOfWork>(provider =>
            {
                var context = provider.GetRequiredService<AppDbContext>();
                var uow = new InMemoryUnitOfWork(context);
                return uow;
            });

        });

        _scope = Factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        Client = Factory.CreateClient();
        TestUserId = Guid.NewGuid();
        InMemoryUow = (InMemoryUnitOfWork)_scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        // Seed initial test data
        await SeedTestDataAsync();
    }
    
    private async Task SeedTestDataAsync()
    {
        // Create test user
        var user = new User
        {
            Id = TestUserId,
            Email = "test.user@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "+1234567890",
            PasswordHash = "hashedpass"
        };

        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
    }

    public async Task<Cart> CreateCartWithItemsAsync(
        int itemCount = 1,
        DateOnly? checkIn = null,
        DateOnly? checkOut = null,
        decimal? pricePerNight = null)
    {
        checkIn ??= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        checkOut ??= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(9));
        pricePerNight ??= 150.00m;

        var city = new City
        {
            Name = "city",
            Country = "country",
            PostalCode = "P400"
        };
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Grand Test Hotel",
            City = city,
            Description = "A wonderful test hotel"
        };

        var roomCategory = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Name = "Deluxe Test Suite",
            Description = "Luxurious test suite",
            PricePerNight = pricePerNight.Value,
            Hotel = hotel
        };

        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomCategory = roomCategory,
            RoomCategoryId = roomCategory.Id,
            RoomNumber = "123"
        };

        await DbContext.Hotels.AddAsync(hotel);
        await DbContext.RoomCategories.AddAsync(roomCategory);
        await DbContext.Rooms.AddAsync(room);


        var cart = new Cart
        {
            UserId = TestUserId,
            Items = new List<CartItem>()
        };

        for (int i = 0; i < itemCount; i++)
        {
            cart.Items.Add(new CartItem
            {
                RoomCategoryId = roomCategory.Id,
                RoomCategory = roomCategory,
                CheckIn = checkIn.Value,
                CheckOut = checkOut.Value,
                Quantity = 1
            });
        }

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        return cart;
    }

    public async Task<Cart> CreateMultiHotelCartAsync()
    {
        var city = new City
        {
            Name = "city",
            Country = "country",
            PostalCode = "P400"
        };
        // Hotel 1
        var hotel1 = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Beach Resort",
            City = city
        };

        var roomCat1 = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel1.Id,
            Name = "Ocean View",
            PricePerNight = 250.00m,
            Hotel = hotel1
        };

        // Hotel 2
        var hotel2 = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Mountain Lodge",
            City = city
        };

        var roomCat2 = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel2.Id,
            Name = "Ski-in Suite",
            PricePerNight = 350.00m,
            Hotel = hotel2
        };
        var room1 = new Room
        {
            Id = Guid.NewGuid(),
            RoomCategory = roomCat1,
            RoomCategoryId = roomCat1.Id,
            RoomNumber = "1234"
        };
        var room2 = new Room
        {
            Id = Guid.NewGuid(),
            RoomCategory = roomCat2,
            RoomCategoryId = roomCat2.Id,
            RoomNumber = "123"
        };
        await DbContext.Rooms.AddRangeAsync(room1, room2);
        await DbContext.Hotels.AddRangeAsync(hotel1, hotel2);
        await DbContext.RoomCategories.AddRangeAsync(roomCat1, roomCat2);

        var cart = new Cart
        {
            UserId = TestUserId,
            Items = new List<CartItem>
                {
                    new()
                    {
                        RoomCategoryId = roomCat1.Id,
                        RoomCategory = roomCat1,
                        CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
                        CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(12)),
                        Quantity = 1
                    },
                    new()
                    {
                        RoomCategoryId = roomCat2.Id,
                        RoomCategory = roomCat2,
                        CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(15)),
                        CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(17)),
                        Quantity = 1
                    }
                }
        };

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        return cart;
    }

    public async Task<Cart> CreateCartWithDiscountAsync()
    {
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Discount Hotel"
        };
        var discount = new Discount
        {
            DiscountPercentage = 20,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(30),
        };


        var roomCategory = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Name = "Standard Room",
            PricePerNight = 200.00m,
            Hotel = hotel,
            Discounts = [discount]
        };
        discount.RoomCategory = roomCategory;
        discount.RoomCategoryId = roomCategory.Id;
        var room = new Room
        {
            Id = Guid.NewGuid(),
            RoomCategory = roomCategory,
            RoomCategoryId = roomCategory.Id,
            RoomNumber = "123"
        };

        await DbContext.Discounts.AddAsync(discount);
        await DbContext.Rooms.AddAsync(room);
        await DbContext.Hotels.AddAsync(hotel);
        await DbContext.RoomCategories.AddAsync(roomCategory);

        var cart = new Cart
        {
            UserId = TestUserId,
            Items = new List<CartItem>
                {
                    new()
                    {
                        RoomCategoryId = roomCategory.Id,
                        RoomCategory = roomCategory,
                        CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                        CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(8)),
                        Quantity = 1
                    }
                }
        };

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        return cart;
    }

    public async Task<Cart> CreateCartWithUnavailableRoomAsync()
    {
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Popular Hotel"
        };

        var roomCategory = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Name = "Popular Suite",
            PricePerNight = 300.00m,
            Hotel = hotel,
        };

        // Create bookings that occupy all rooms
        for (int i = 0; i < 5; i++)
        {
            var booking = new Booking
            {
                HotelId = hotel.Id,
                UserId = Guid.NewGuid(), // Different user
                CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                Rooms = new List<Room>
                    {
                        new() { RoomCategoryId = roomCategory.Id }
                    }
            };

            await DbContext.Bookings.AddAsync(booking);
        }

        await DbContext.Hotels.AddAsync(hotel);
        await DbContext.RoomCategories.AddAsync(roomCategory);
        await DbContext.SaveChangesAsync();

        var cart = new Cart
        {
            UserId = TestUserId,
            Items = new List<CartItem>
                {
                    new()
                    {
                        RoomCategoryId = roomCategory.Id,
                        RoomCategory = roomCategory,
                        CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                        CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(5)),
                        Quantity = 1
                    }
                }
        };

        await DbContext.Carts.AddAsync(cart);
        await DbContext.SaveChangesAsync();

        return cart;
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }

    public async Task DisposeAsync()
    {
        _scope?.Dispose();
        Client?.Dispose();

        if (Factory != null)
        {
            await Factory.DisposeAsync();
        }
    }
}
