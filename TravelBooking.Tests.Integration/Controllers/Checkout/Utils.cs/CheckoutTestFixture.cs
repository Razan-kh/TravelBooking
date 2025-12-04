
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using System.Data;
using TravelBooking.Domain.Cities.Entities;
using BookingSystem.IntegrationTests.Checkout.Utils;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Domain.Bookings.Repositories;

namespace BookingSystem.IntegrationTests.Checkout.Utils;

public class CheckoutTestFixture : IAsyncLifetime
{
    public ApiTestFactory Factory { get; private set; } = null!;
    public HttpClient Client { get; private set; } = null!;
    public TestEmailService EmailService { get; private set; } = null!;
    public TestPaymentService PaymentService { get; private set; } = null!;
    public TestPdfService PdfService { get; private set; } = null!;
    public AppDbContext DbContext { get; private set; } = null!;
    public Guid TestUserId { get; private set; }

    private IServiceScope _scope = null!;

    public async Task InitializeAsync()
    {
        Factory = new ApiTestFactory();
        Factory.SetInMemoryDbName($"CheckoutTests_{Guid.NewGuid()}");

        // Override external services with test implementations
        Factory.AddServiceConfiguration(services =>
        {
            // Override services (don't remove, just override)
            /*
            services.AddSingleton<IEmailService>(new TestEmailService());
            services.AddSingleton<IPaymentService>(new TestPaymentService());
            services.AddSingleton<IPdfService>(new TestPdfService());
                DebugServiceRegistrations(services);
*/
            
            // Remove external services
            RemoveService<IEmailService>(services);
            RemoveService<IPaymentService>(services);
            RemoveService<IPdfService>(services);

            // Add test implementations
            EmailService = new TestEmailService();
            PaymentService = new TestPaymentService();
            PdfService = new TestPdfService();

            services.AddSingleton<IEmailService>(EmailService);
            services.AddSingleton<IPaymentService>(PaymentService);
            services.AddSingleton<IPdfService>(PdfService);
            
        });

        Client = Factory.CreateClient();
        TestUserId = Guid.NewGuid();

        // Create service scope
        _scope = Factory.Services.CreateScope();
        DbContext = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Seed initial test data
        await SeedTestDataAsync();
    }
private void DebugServiceRegistrations(IServiceCollection services)
{
    Console.WriteLine("=== Service Registrations ===");
    
    var serviceTypes = new[]
    {
        typeof(ICartService),
        typeof(IPaymentService),
        typeof(IBookingService),
        typeof(IPdfService),
        typeof(IEmailService),
        typeof(IUserRepository),
        typeof(IUnitOfWork),
        typeof(IDiscountService),
        typeof(IRoomAvailabilityService),
        typeof(IBookingRepository)
    };
    
    foreach (var serviceType in serviceTypes)
    {
        var registrations = services.Where(s => s.ServiceType == serviceType).ToList();
        Console.WriteLine($"{serviceType.Name}: {registrations.Count} registrations");
        foreach (var reg in registrations)
        {
            Console.WriteLine($"  - Lifetime: {reg.Lifetime}, Implementation: {reg.ImplementationType?.Name ?? "Instance"}");
        }
    }
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

        await DbContext.Hotels.AddAsync(hotel);
        await DbContext.RoomCategories.AddAsync(roomCategory);

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
                Quantity = 1 + i % 2 // Alternate between 1 and 2
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
                        Quantity = 2
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

        var roomCategory = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Name = "Standard Room",
            PricePerNight = 200.00m,
            Hotel = hotel,
            Discounts = new List<Discount>
                {
                    new()
                    {
                        DiscountPercentage = 20,
                        StartDate = DateTime.UtcNow.AddDays(-1),
                        EndDate = DateTime.UtcNow.AddDays(30),

                    }
                }
        };

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
