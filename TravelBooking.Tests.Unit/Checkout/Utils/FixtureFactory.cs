
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Reviews.Entities;

using TravelBooking.Domain.Bookings.Entities;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace TravelBooking.Tests.Application.Checkout.Utils;

public static class FixtureFactory
{
    public static Cart CreateTestCart(this IFixture fixture, int itemCount = 2)
    {
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Items = new List<CartItem>()
        };

        for (int i = 0; i < itemCount; i++)
        {
            var hotel = new Hotel
            {
                Id = Guid.NewGuid(),
                Name = $"Hotel {i}",
                City = new City { Id = Guid.NewGuid(), Name = $"City {i}", Country = $"Country{i}", PostalCode = $"PostalCode {i}" }
            };

            var roomCategory = new RoomCategory
            {
                Id = Guid.NewGuid(),
                Name = $"Room Category {i}",
                PricePerNight = 100 + (i * 50),
                Hotel = hotel,
                HotelId = hotel.Id
            };

            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                RoomCategory = roomCategory,
                RoomCategoryId = roomCategory.Id,
                Quantity = 1,
                CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
                CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                Cart = cart,
                CartId = cart.Id
            };

            cart.Items.Add(cartItem);
        }

        return cart;
    }
    public static User CreateValidUser(this IFixture fixture, string email = "test@example.com")
    {
        return fixture.Build<User>()
            .With(u => u.Email, email)
            .Without(u => u.Bookings)
            .Create();
    }
    public static List<Booking> CreateValidBookings(this IFixture fixture, int count = 2)
    {
        return fixture.Build<Booking>()
            .Without(b => b.User)
            .With(b => b.CheckInDate, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
            .With(b => b.CheckOutDate, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
            .CreateMany(count)
            .ToList();
    }
}