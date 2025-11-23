using AutoFixture;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Rooms.Entities;

public class EntityCustomization : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<City>(c => c
            .With(x => x.Id, Guid.NewGuid())
            .Without(x => x.Hotels)
        );

        fixture.Customize<Hotel>(c => c
            .With(x => x.Id, Guid.NewGuid())
            .Without(x => x.Bookings)
            .Without(x => x.Reviews)
            .Without(x => x.Owner)
            .Without(x => x.RoomCategories)
        );

        fixture.Customize<CreateHotelDto>(c => c
            .With(x => x.Name, "Test Hotel")
            .With(x => x.CityId, Guid.NewGuid())
            .With(x => x.OwnerId, Guid.NewGuid())
            .With(x => x.StarRating, 5)
            .With(x => x.TotalRooms, 100)
        );

        fixture.Customize<Room>(c => c
            .With(x => x.Id, Guid.NewGuid())
            .Without(x => x.Bookings)
            .Without(x => x.RoomCategory)
        );
    }
}