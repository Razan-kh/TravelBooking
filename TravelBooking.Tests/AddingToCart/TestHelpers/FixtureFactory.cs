using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using TravelBooking.Application.AddingToCart.Handlers;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Discounts.Entities;

using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using AutoFixture;
using AutoFixture.AutoMoq;


using AutoFixture;
using AutoFixture.AutoMoq;
using global::TravelBooking.Domain.Payments.Entities;
using global::TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Tests.AddingToCart.TestHelpers;

public static class FixtureFactory
{
    public static IFixture Create()
    {
        var fixture = new Fixture();
        
        // Configure AutoMoq first
        fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        
        // Handle circular references
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        // Fix DateOnly creation
        fixture.Customize<DateOnly>(c => c.FromFactory<DateTime>(DateOnly.FromDateTime));
        
        // Configure all entities to avoid circular references
        ConfigureEntityCustomizations(fixture);
        
        return fixture;
    }
    
    private static void ConfigureEntityCustomizations(IFixture fixture)
    {
        // RoomCategory customization
        fixture.Customize<RoomCategory>(composer => composer
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .With(rc => rc.PricePerNight, 100.0m)
            .With(rc => rc.AdultsCapacity, 2)
            .With(rc => rc.ChildrenCapacity, 1)
            .With(rc => rc.Name, "Standard Room")
            .With(rc => rc.RoomType, RoomType.Standard));
        
        // Hotel customization
        fixture.Customize<Hotel>(composer => composer
            .Without(h => h.City)
            .Without(h => h.Owner)
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews)
            .Without(h => h.Gallery)
            .Without(h => h.Bookings)
            .With(h => h.Name, "Test Hotel")
            .With(h => h.StarRating, 4)
            .With(h => h.TotalRooms, 50)
            .With(h => h.HotelType, HotelType.Hotel));
        
        // Discount customization
        fixture.Customize<Discount>(composer => composer
            .Without(d => d.RoomCategory)
            .With(d => d.DiscountPercentage, 10.0m)
            .With(d => d.StartDate, DateTime.UtcNow.AddDays(-1))
            .With(d => d.EndDate, DateTime.UtcNow.AddDays(1)));
        
        // Amenity customization
        fixture.Customize<Amenity>(composer => composer
            .Without(a => a.RoomCategories)
            .With(a => a.Name, "WiFi")
            .With(a => a.Description, "Free WiFi"));
        
        // Review customization
        fixture.Customize<Review>(composer => composer
            .Without(r => r.User)
            .Without(r => r.Hotel)
            .With(r => r.Rating, 5)
            .With(r => r.Content, "Great hotel!"));
        
        // GalleryImage customization
        fixture.Customize<GalleryImage>(composer => composer
            .With(g => g.Path, "/images/test.jpg"));
        
        // Booking customization
        fixture.Customize<Booking>(composer => composer
            .Without(b => b.User)
            .Without(b => b.Hotel)
            .Without(b => b.Rooms)
            .With(b => b.GuestRemarks, "Test booking")
            .With(b => b.CheckInDate, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .With(b => b.CheckOutDate, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
            .With(b => b.BookingDate, DateTime.UtcNow));
        
        // User customization
        fixture.Customize<User>(composer => composer
            .Without(u => u.Bookings)
            .With(u => u.Email, "test@example.com")
            .With(u => u.FirstName, "Test")
            .With(u => u.LastName, "User"));
    }
    
    // Helper method to create RoomCategory with discounts
    public static RoomCategory CreateRoomCategoryWithDiscount(this IFixture fixture, decimal pricePerNight = 100.0m)
    {
        return fixture.Build<RoomCategory>()
            .With(r => r.PricePerNight, pricePerNight)
            .With(r => r.Discounts, new List<Discount>
            {
                fixture.Build<Discount>()
                    .Without(d => d.RoomCategory)
                    .With(d => d.DiscountPercentage, 10.0m)
                    .With(d => d.StartDate, DateTime.UtcNow.AddDays(-1))
                    .With(d => d.EndDate, DateTime.UtcNow.AddDays(1))
                    .Create()
            })
            .Without(r => r.Hotel)
            .Without(r => r.Rooms)
            .Without(r => r.Amenities)
            .Create();
    }
    
    // Helper method to create RoomCategory without discounts
    public static RoomCategory CreateRoomCategoryWithoutDiscounts(this IFixture fixture, decimal pricePerNight = 100.0m)
    {
        return fixture.Build<RoomCategory>()
            .With(r => r.PricePerNight, pricePerNight)
            .With(r => r.Discounts, new List<Discount>())
            .Without(r => r.Hotel)
            .Without(r => r.Rooms)
            .Without(r => r.Amenities)
            .Create();
    }
    
    // Other helper methods
    public static AddRoomToCartCommand CreateAddRoomToCartCommand(this IFixture fixture)
    {
        return fixture.Build<AddRoomToCartCommand>()
            .With(c => c.CheckIn, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .With(c => c.CheckOut, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
            .With(c => c.Quantity, 1)
            .Create();
    }
    
    public static Cart CreateValidCart(this IFixture fixture, int itemCount = 2)
    {
        var cart = fixture.Build<Cart>()
            .Without(c => c.Items)
            .Create();
        
        var items = new List<CartItem>();
        for (int i = 0; i < itemCount; i++)
        {
            var roomCategory = fixture.CreateRoomCategoryWithDiscount(100.0m + (i * 50));
            
            var cartItem = fixture.Build<CartItem>()
                .With(ci => ci.RoomCategory, roomCategory)
                .With(ci => ci.RoomCategoryId, roomCategory.Id)
                .With(ci => ci.Cart, cart)
                .With(ci => ci.CartId, cart.Id)
                .With(ci => ci.Quantity, 1)
                .With(ci => ci.CheckIn, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                .With(ci => ci.CheckOut, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
                .Without(ci => ci.Cart)
                .Create();
                
            items.Add(cartItem);
        }
        
        cart.Items = items;
        return cart;
    }
    
    public static List<Booking> CreateValidBookings(this IFixture fixture, int count = 2)
    {
        return fixture.Build<Booking>()
            .Without(b => b.User)
            .Without(b => b.Hotel)
            .Without(b => b.Rooms)
            .With(b => b.PaymentDetails, fixture.Build<PaymentDetails>()
                .With(p => p.Amount, 200.0m)
                .With(p => p.PaymentMethod, PaymentMethod.Card)
                .With(p => p.PaymentDate, DateTime.UtcNow)
                .Create())
            .CreateMany(count)
            .ToList();
    }
}

/*
public static class FixtureFactory
{
    public static IFixture Create()
    {
        var fixture = new Fixture();

        // 1. Configure AutoMoq FIRST
        fixture.Customize(new AutoMoqCustomization());

        // 2. Remove throwing behavior and add recursion handling
        fixture.Behaviors.Remove(fixture.Behaviors.OfType<ThrowingRecursionBehavior>().Single());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // 3. Fix DateOnce and for all - use a factory that always creates valid dates
        fixture.Customize<DateOnly>(c => c.FromFactory<DateTime>(DateOnly.FromDateTime));

        // 4. Register DateTime to always create valid dates
        fixture.Register<DateTime>(() => DateTime.Today.AddDays(new Random().Next(1, 30)));

        return fixture;
    }

    // SIMPLE manual creation methods - no AutoFixture in these
    public static Cart CreateValidCart(int itemCount = 2)
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Items = new List<CartItem>()
        };

        for (int i = 0; i < itemCount; i++)
        {
            var hotelId = Guid.NewGuid();

            var roomCategory = new RoomCategory
            {
                Id = Guid.NewGuid(),
                Name = $"Room Type {i + 1}",
                PricePerNight = 100.0m + (i * 50),
                HotelId = hotelId,
                Description = $"Test Room {i + 1}",
                AdultsCapacity = 2
            };

            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                RoomCategory = roomCategory,
                RoomCategoryId = roomCategory.Id,
                CartId = cart.Id,
                Quantity = 1,
                CheckIn = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                CheckOut = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            };

            cart.Items.Add(cartItem);
        }

        return cart;
    }

    public static User CreateValidUser(string email = "test@example.com")
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            FirstName = "Test",
            LastName = "User",
        };
    }

    public static List<Booking> CreateValidBookings(int count = 2)
    {
        var bookings = new List<Booking>();

        for (int i = 0; i < count; i++)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                HotelId = Guid.NewGuid(),
                CheckInDate = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
                CheckOutDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
                BookingDate = DateTime.UtcNow,
                GuestRemarks = $"Test booking {i + 1}",
                PaymentDetails = new PaymentDetails
                {
                    Id = Guid.NewGuid(),
                    Amount = 200.0m + (i * 50),
                    PaymentNumber = i + 1,
                    PaymentDate = DateTime.UtcNow,
                    PaymentMethod = PaymentMethod.Card,
                    BookingId = Guid.NewGuid() // This will be set to the booking ID below
                },
                Rooms = new List<Room>(),
            };

            // Set the BookingId reference after creating the booking
            booking.PaymentDetails.BookingId = booking.Id;

            bookings.Add(booking);
        }

        return bookings;
    }
}*/
/*
    public static AddRoomToCartCommand CreateAddRoomToCartCommand()
    {
        return new AddRoomToCartCommand
        {
            UserId = Guid.NewGuid(),
            RoomCategoryId = Guid.NewGuid(),
            CheckIn = DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.Today.AddDays(3)),
            Quantity = 1
        };
    }
}
/*
public static class FixtureFactory
{
    public static IFixture Create()
    {
        var fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        // Handle circular references
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
        fixture.Behaviors.Add(new OmitOnRecursionBehavior(3)); // Limit recursion depth

        // Fix DateOnly creation - this is crucial
        fixture.Register<DateOnly>(() => DateOnly.FromDateTime(DateTime.Today.AddDays(fixture.Create<int>() % 30 + 1)));

        // Customize complex entities to avoid circular references
        fixture.Customize<Cart>(composer => composer
            .Without(c => c.Items));

        fixture.Customize<CartItem>(composer => composer
            .Without(ci => ci.Cart)
            .Without(ci => ci.RoomCategory)
            .With(ci => ci.CheckIn, () => DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .With(ci => ci.CheckOut, () => DateOnly.FromDateTime(DateTime.Today.AddDays(3))));

        fixture.Customize<RoomCategory>(composer => composer
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Discounts));

        fixture.Customize<Hotel>(composer => composer
            .Without(h => h.City)
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews));

        fixture.Customize<User>(composer => composer
            .Without(u => u.Bookings));

        fixture.Customize<Booking>(composer => composer
            .Without(b => b.User)
            .With(b => b.CheckInDate, () => DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .With(b => b.CheckOutDate, () => DateOnly.FromDateTime(DateTime.Today.AddDays(3))));

        return fixture;
    }

    public static AddRoomToCartCommand CreateAddRoomToCartCommand(this IFixture fixture)
    {
        return fixture.Build<AddRoomToCartCommand>()
            .With(c => c.CheckIn, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .With(c => c.CheckOut, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
            .With(c => c.Quantity, 1)
            .Create();
    }

    public static Cart CreateCart(this IFixture fixture, int itemCount = 2)
    {
        var cart = fixture.Build<Cart>()
            .Without(c => c.Items)
            .Create();

        var items = new List<CartItem>();
          
    for (int i = 0; i < itemCount; i++)
    {
        // Create a simple hotel without any navigation properties
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = $"Hotel {i + 1}",
            CityId = Guid.NewGuid() // Just set the ID, not the navigation property
        };
            var roomCategory = fixture.Build<RoomCategory>()
                .With(rc => rc.Hotel, hotel)
                .With(rc => rc.HotelId, hotel.Id)
                .Without(rc => rc.Discounts)
                .Create();

            var cartItem = fixture.Build<CartItem>()
                .With(ci => ci.RoomCategory, roomCategory)
                .With(ci => ci.RoomCategoryId, roomCategory.Id)
                .With(ci => ci.Cart, cart)
                .With(ci => ci.CartId, cart.Id)
                .With(ci => ci.CheckIn, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
                .With(ci => ci.CheckOut, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
                .With(ci => ci.Quantity, 1)
                .Create();

            items.Add(cartItem);
        }

        cart.Items = items;
        return cart;
    }
}
*/
/*
public static class FixtureFactory
{
    public static IFixture Create()
    {
        var fixture = new Fixture();

        // AutoMoq (required)
        fixture.Customize(new AutoMoqCustomization
        {
            ConfigureMembers = false // prevent AutoFixture from populating Mock internals
        });

        // Remove recursion exceptions
        fixture.Behaviors.OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => fixture.Behaviors.Remove(b));

        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Fix DateOnly generation
        fixture.Register(() =>
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(new Random().Next(1, 30)))
        );

        // Replace ICollection<T> with List<T> (safe for EF Core entity navigation)
        fixture.Customizations.Add(
            new TypeRelay(typeof(ICollection<>), typeof(List<>))
        );

        return fixture;
    }
}
*/