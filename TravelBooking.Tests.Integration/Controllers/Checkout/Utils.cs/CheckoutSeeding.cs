using TravelBooking.Infrastructure.Persistence;
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
public class CheackoutSeeding
{
        public static async Task<Cart> CreateCartWithDiscountAsync(AppDbContext dbContext, Guid testUserId)
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

        await dbContext.Discounts.AddAsync(discount);
        await dbContext.Rooms.AddAsync(room);
        await dbContext.Hotels.AddAsync(hotel);
        await dbContext.RoomCategories.AddAsync(roomCategory);

        var cart = new Cart
        {
            UserId = testUserId,
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

        await dbContext.Carts.AddAsync(cart);
        await dbContext.SaveChangesAsync();

        return cart;
    }

    public static async Task<Cart> CreateCartWithUnavailableRoomAsync(AppDbContext dbContext, Guid testUserId)
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

            await dbContext.Bookings.AddAsync(booking);
        }

        await dbContext.Hotels.AddAsync(hotel);
        await dbContext.RoomCategories.AddAsync(roomCategory);
        await dbContext.SaveChangesAsync();

        var cart = new Cart
        {
            UserId = testUserId,
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

        await dbContext.Carts.AddAsync(cart);
        await dbContext.SaveChangesAsync();

        return cart;
    }

    private static void RemoveService<T>(IServiceCollection services)
    {
        var descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T));
        if (descriptor != null)
            services.Remove(descriptor);
    }
}