using AutoFixture;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Tests.Integration.Helpers;

public static class FixtureConfiguration
{
    public static void ConfigureHomeControllerFixture(this IFixture fixture)
    {
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Customize<Hotel>(composer => composer
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews)
            .Without(h => h.Gallery)
            .Without(h => h.Bookings)
            .Without(h => h.City)
            .Without(h => h.Owner));

        fixture.Customize<City>(composer => composer
            .Without(c => c.Hotels));

        fixture.Customize<Booking>(composer => composer
            .Without(b => b.User)
            .Without(b => b.Hotel)
            .Without(b => b.Rooms));
    }
}