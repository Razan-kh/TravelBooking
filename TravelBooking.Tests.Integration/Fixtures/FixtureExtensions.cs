using AutoFixture;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Users.Entities;

namespace TravelBooking.Tests.Integration.Extensions;

public static class FixtureExtensions
{
    // City without Hotels
    public static City CreateCityWithoutHotels(this IFixture fixture)
    {
        return fixture.Build<City>()
            .Without(c => c.Hotels)
            .Create();
    }

    // Hotel without Bookings, Reviews, Rooms, Gallery
    public static Hotel CreateHotelMinimal(this IFixture fixture, Guid cityId)
    {
        return fixture.Build<Hotel>()
            .Without(h => h.Bookings)
            .Without(h => h.Reviews)
            .Without(h => h.RoomCategories)
            .Without(h => h.Gallery)
            .With(h => h.CityId, cityId)
            .Create();
    }

    // RoomCategory without Rooms, Amenities, Discounts, Hotel
    public static RoomCategory CreateRoomCategoryMinimal(this IFixture fixture)
    {
        return fixture.Build<RoomCategory>()
            .Without(rc => rc.Rooms)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Create();
    }

    // Room without Bookings, Gallery
    public static Room CreateRoomMinimal(this IFixture fixture, RoomCategory category)
    {
        return fixture.Build<Room>()
            .Without(r => r.Bookings)
            .With(r => r.RoomCategory, category)
            .Without(r => r.Gallery)
            .Create();
    }

    // Room with single GalleryImage
    public static Room CreateRoomWithGallery(this IFixture fixture, RoomCategory category)
    {
        var galleryImage = fixture.Build<GalleryImage>().Create();
        return fixture.Build<Room>()
            .Without(r => r.Bookings)
            .With(r => r.RoomCategory, category)
            .With(r => r.Gallery, new List<GalleryImage> { galleryImage })
            .Create();
    }

    // Owner without Hotels
    public static Owner CreateOwnerMinimal(this IFixture fixture)
    {
        return fixture.Build<Owner>()
            .Without(o => o.Hotels)
            .Create();
    }

    public static GalleryImage CreateGalleryMinimal(this IFixture fixture)
    {
        return fixture.Build<GalleryImage>().Create();
    }

    public static User CreateUserMinimal(this IFixture fixture, string? email = null, string? passwordHash = null)
    {
        return fixture.Build<User>()
            .With(u => u.Email, email ?? fixture.Create<string>() + "@example.com")
            .With(u => u.PasswordHash, passwordHash ?? "hashedpassword") // adjust as needed
            .Without(u => u.Bookings)
            .Create();
    }

}