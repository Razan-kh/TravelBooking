using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Bookings.Entities;
using AutoFixture;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Users.Enums;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Tests.Integration.Builders;

public class TestDataBuilder
{
    private readonly Fixture _fixture;

    public TestDataBuilder()
    {
        _fixture = new Fixture();
        
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        
        ConfigureCustomizations();
    }

    private void ConfigureCustomizations()
    {
        _fixture.Customize<Booking>(composer => composer
            .Without(b => b.User)
            .Without(b => b.Hotel)
            .Without(b => b.Rooms));

        _fixture.Customize<Room>(composer => composer
            .Without(r => r.Bookings)
            .Without(r => r.RoomCategory)
            .Without(r => r.Gallery));

        _fixture.Customize<RoomCategory>(composer => composer
            .Without(rc => rc.Rooms)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts));

        _fixture.Customize<Hotel>(composer => composer
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews)
            .Without(h => h.Gallery)
            .Without(h => h.Bookings)
            .Without(h => h.City)
            .Without(h => h.Owner));

        _fixture.Customize<User>(composer => composer
            .Without(u => u.Bookings));

        // Configure DateTime properties
        _fixture.Customize<DateTime>(composer => composer
            .FromFactory(() => DateTime.UtcNow));

        _fixture.Customize<DateOnly>(composer => composer
            .FromFactory(() => DateOnly.FromDateTime(DateTime.UtcNow)));
    }

    public T Create<T>() => _fixture.Create<T>();

    public IFixture Fixture => _fixture;

    public User CreateUser(Guid? id = null, string email = "test.user@example.com", UserRole role = UserRole.User)
    {
        return _fixture.Build<User>()
            .With(u => u.Id, id ?? Guid.NewGuid())
            .With(u => u.Email, email)
            .With(u => u.Role, role)
            .Create();
    }

    public Hotel CreateHotel(Guid? id = null, string name = "Grand Plaza Hotel", int starRating = 5)
    {
        return _fixture.Build<Hotel>()
            .With(h => h.Id, id ?? Guid.NewGuid())
            .With(h => h.Name, name)
            .With(h => h.StarRating, starRating)
            .With(h => h.TotalRooms, 100)
            .With(h => h.HotelType, HotelType.Resort)
            .Without(h => h.Bookings)
            .Without(h => h.City)
            .Without(h => h.Gallery)
            .Without(h => h.Owner)
            .Create();
    }

    public RoomCategory CreateRoomCategory(Guid hotelId, Guid? id = null, string name = "Deluxe Suite")
    {
        return _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, id ?? Guid.NewGuid())
            .With(rc => rc.HotelId, hotelId)
            .With(rc => rc.Name, name)
            .With(rc => rc.AdultsCapacity, 2)
            .With(rc => rc.ChildrenCapacity, 2)
            .With(rc => rc.PricePerNight, 250.00m)
            .With(rc => rc.RoomType, RoomType.Suite)
            .Create();
    }

    public Room CreateRoom(Guid roomCategoryId, Guid? id = null, string roomNumber = "101")
    {
        return _fixture.Build<Room>()
            .With(r => r.Id, id ?? Guid.NewGuid())
            .With(r => r.RoomNumber, roomNumber)
            .With(r => r.RoomCategoryId, roomCategoryId)
            .Create();
    }

    public Booking CreateBooking(Guid userId, Guid hotelId, DateOnly? checkIn = null, DateOnly? checkOut = null)
    {
        var defaultCheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        var defaultCheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(9));

        return _fixture.Build<Booking>()
            .With(b => b.UserId, userId)
            .With(b => b.HotelId, hotelId)
            .With(b => b.CheckInDate, checkIn ?? defaultCheckIn)
            .With(b => b.CheckOutDate, checkOut ?? defaultCheckOut)
            .With(b => b.GuestRemarks, "Early check-in requested")
            .With(b => b.PaymentDetails, CreatePaymentDetails(500.00m, PaymentMethod.Card))
            .Create();
    }

    public PaymentDetails CreatePaymentDetails(decimal amount = 500.00m, PaymentMethod method = PaymentMethod.Card)
    {
        return _fixture.Build<PaymentDetails>()
            .With(pd => pd.Amount, amount)
            .With(pd => pd.PaymentMethod, method)
            .Create();
    }
}