using Bogus;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TravelBooking.Domain;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Domain.Users.Enums;

namespace TravelBooking.Infrastructure.Persistence.Seeders;

public static class BogusSeeder
{
    // Option A sizes
    public const int CitiesCount = 10;
    public const int OwnersCount = 20;
    public const int HotelsCount = 30;
    public const int RoomCategoriesCount = 60;
    public const int RoomsCount = 300;
    public const int UsersCount = 150;
    public const int BookingsCount = 150;
    public const int ReviewsCount = 200;
    public const int DiscountsCount = 60;
    public const int AmenitiesCount = 20;

    // Deterministic seed for repeatability
    private static readonly int RandomizerSeed = 123;

    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        if (db == null) throw new ArgumentNullException(nameof(db));

        // Quick guard: if data exists, skip seeding
        if (await db.Cities.AnyAsync(ct) ||
            await db.Hotels.AnyAsync(ct) ||
            await db.Users.AnyAsync(ct))
        {
            return;
        }

        // Make Bogus deterministic
        Randomizer.Seed = new Random(RandomizerSeed);

        // --- 1. Cities ---
        var cityFaker = new Faker<City>("en")
            .RuleFor(c => c.Id, f => Guid.NewGuid())
            .RuleFor(c => c.Name, f => f.Address.City())
            .RuleFor(c => c.Country, f => f.Address.Country())
            .RuleFor(c => c.PostalCode, f => f.Address.ZipCode())
            .RuleFor(c => c.ThumbnailUrl, f => $"/images/cities/{f.System.FileName("png")}");

        var cities = cityFaker.Generate(CitiesCount);

        // --- 2. Owners ---
        var ownerFaker = new Faker<Owner>("en")
            .RuleFor(o => o.Id, f => Guid.NewGuid())
            .RuleFor(o => o.Email, f => f.Internet.Email())
            .RuleFor(o => o.FirstName, f => f.Name.FirstName())
            .RuleFor(o => o.LastName, f => f.Name.LastName())
            .RuleFor(o => o.PhoneNumber, f => f.Phone.PhoneNumber());

        var owners = ownerFaker.Generate(OwnersCount);

        // --- 3. Amenities ---
        var amenityNames = new[]
        {
                "Free WiFi", "Swimming Pool", "Parking", "Spa", "Gym",
                "Airport Shuttle", "Breakfast Included", "Pet Friendly",
                "Restaurant", "Bar", "24h Front Desk", "AC", "Heating",
                "Elevator", "Business Center", "Concierge", "Laundry",
                "Room Service", "Wheelchair Access", "Sauna"
            }.Take(AmenitiesCount).ToList();

        var amenityFaker = new Faker<Amenity>("en")
            .RuleFor(a => a.Id, f => Guid.NewGuid())
            .RuleFor(a => a.Name, f => f.PickRandom(amenityNames))
            .RuleFor(a => a.Description, f => f.Lorem.Sentence(6));

        var amenities = amenityFaker.Generate(AmenitiesCount);

        // --- 4. Hotels ---
        var hotelFaker = new Faker<Hotel>("en")
            .RuleFor(h => h.Id, f => Guid.NewGuid())
            .RuleFor(h => h.Name, f => $"{f.Company.CompanyName()} Hotel")
            .RuleFor(h => h.Description, f => f.Lorem.Paragraph())
            .RuleFor(h => h.ThumbnailUrl, f => $"/images/hotels/{f.System.FileName("png")}")
            .RuleFor(h => h.Email, f => f.Internet.Email())
            .RuleFor(h => h.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(h => h.StarRating, f => f.Random.Int(1, 5))
            .RuleFor(h => h.Longitude, f => f.Address.Longitude())
            .RuleFor(h => h.Latitude, f => f.Address.Latitude())
            .RuleFor(h => h.TotalRooms, f => f.Random.Int(10, 300))
            .RuleFor(h => h.HotelType, f => f.PickRandom<HotelType>());
        //  .RuleFor(h => h.CreatedAt, f => DateTime.UtcNow.AddDays(-f.Random.Int(1, 1000)))
        //    .RuleFor(h => h.UpdatedAt, (f, h) => h.CreatedAt.AddDays(f.Random.Int(0, 200)));

        var hotels = new List<Hotel>();
        for (int i = 0; i < HotelsCount; i++)
        {
            var hotel = hotelFaker.Generate();
            hotel.CityId = cities[fakerIndex(cities.Count)].Id;
            hotel.OwnerId = owners[fakerIndex(owners.Count)].Id;
            hotels.Add(hotel);
        }

        // --- 5. RoomCategories ---
        var rcFaker = new Faker<RoomCategory>("en")
            .RuleFor(rc => rc.Id, f => Guid.NewGuid())
            .RuleFor(rc => rc.Name, f => f.Commerce.ProductName())
            .RuleFor(rc => rc.Description, f => f.Lorem.Sentence(8))
            .RuleFor(rc => rc.AdultsCapacity, f => f.Random.Int(1, 4))
            .RuleFor(rc => rc.ChildrenCapacity, f => f.Random.Int(0, 3))
            .RuleFor(rc => rc.PricePerNight, f => f.Random.Decimal(30, 500))
            .RuleFor(rc => rc.RoomType, f => f.PickRandom<RoomType>())
            .RuleFor(rc => rc.HotelId, f => hotels[fakerIndex(hotels.Count)].Id);

        var roomCategories = rcFaker.Generate(RoomCategoriesCount);

        // --- 6. Rooms ---
        var roomFaker = new Faker<Room>("en")
            .RuleFor(r => r.Id, f => Guid.NewGuid())
            .RuleFor(r => r.RoomNumber, f => $"{f.Random.Int(100, 999)}")
            .RuleFor(r => r.RoomCategoryId, f => roomCategories[fakerIndex(roomCategories.Count)].Id);
        //   .RuleFor(r => r.CreatedAt, f => DateTime.UtcNow.AddDays(-f.Random.Int(1, 1000)))
        //   .RuleFor(r => r.UpdatedAt, (f, r) => r.CreatedAt.AddDays(f.Random.Int(0, 200)));

        var rooms = roomFaker.Generate(RoomsCount);

        // --- 7. Users ---
        var userFaker = new Faker<User>("en")
            .RuleFor(u => u.Id, f => Guid.NewGuid())
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => "hashed-placeholder") // replace with real hashing in production
            .RuleFor(u => u.PhoneNumber, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.Role, f => f.PickRandom<UserRole>());

        var users = userFaker.Generate(UsersCount);

        // --- 8. Discounts (on some RoomCategories) ---
        var discountFaker = new Faker<Discount>("en")
            .RuleFor(d => d.Id, f => Guid.NewGuid())
            .RuleFor(d => d.DiscountPercentage, f => f.Random.Decimal(5, 30))
            .RuleFor(d => d.StartDate, f => DateTime.UtcNow.AddDays(-f.Random.Int(0, 30)))
            .RuleFor(d => d.EndDate, (f, d) => d.StartDate.AddDays(f.Random.Int(7, 60)))
            .RuleFor(d => d.RoomCategoryId, f => roomCategories[fakerIndex(roomCategories.Count)].Id);

        var discounts = discountFaker.Generate(DiscountsCount);

        // --- 9. Reviews ---
        var reviewFaker = new Faker<Review>("en")
            .RuleFor(rv => rv.Id, f => Guid.NewGuid())
            .RuleFor(rv => rv.UserId, f => users[fakerIndex(users.Count)].Id)
            .RuleFor(rv => rv.HotelId, f => hotels[fakerIndex(hotels.Count)].Id)
            .RuleFor(rv => rv.Content, f => f.Lorem.Sentence(10))
            .RuleFor(rv => rv.Rating, f => f.Random.Int(1, 5));
        //    .RuleFor(rv => rv.CreatedAt, f => DateTime.UtcNow.AddDays(-f.Random.Int(1, 400)));

        var reviews = reviewFaker.Generate(ReviewsCount);

        // --- 10. Bookings & PaymentDetails ---
        var bookingFaker = new Faker<Booking>("en")
            .RuleFor(b => b.Id, f => Guid.NewGuid())
            .RuleFor(b => b.UserId, f => users[fakerIndex(users.Count)].Id)
            .RuleFor(b => b.HotelId, f => hotels[fakerIndex(hotels.Count)].Id)
            .RuleFor(b => b.GuestRemarks, f => f.Lorem.Sentence(6))
            .RuleFor(b => b.CheckInDate, f => DateOnly.FromDateTime(DateTime.UtcNow.AddDays(f.Random.Int(1, 60))))
            .RuleFor(b => b.CheckOutDate, (f, b) => DateOnly.FromDateTime(b.CheckInDate.ToDateTime(TimeOnly.MinValue).AddDays(f.Random.Int(1, 10))))
            .RuleFor(b => b.BookingDate, f => DateTime.UtcNow.AddDays(-f.Random.Int(1, 60)));

        var bookings = bookingFaker.Generate(BookingsCount);

        // Create PaymentDetails list (one-to-one with bookings)
        var payments = new List<PaymentDetails>();
        foreach (var booking in bookings)
        {
            var nights = (booking.CheckOutDate.ToDateTime(TimeOnly.MinValue) - booking.CheckInDate.ToDateTime(TimeOnly.MinValue)).Days;
            if (nights <= 0) nights = 1;
            // pick a random room category linked to hotel to estimate price
            var possibleCategories = roomCategories.Where(rc => rc.HotelId == booking.HotelId).ToList();
            var samplePrice = possibleCategories.Any() ? possibleCategories[fakerIndex(possibleCategories.Count)].PricePerNight : 100m;
            var amount = samplePrice * nights;

            var payment = new PaymentDetails
            {
                Id = Guid.NewGuid(),
                Amount = amount,
                PaymentNumber = RandomNumber(100000, 999999),
                PaymentDate = booking.BookingDate.AddHours(1),
                PaymentMethod = RandomEnum<PaymentMethod>(),
                BookingId = booking.Id
            };
            payments.Add(payment);
        }

        // --- 11. Booking ↔ Rooms Many-to-Many (assign 1-3 rooms per booking) ---
        var bookingRoomPairs = new List<(Guid bookingId, Guid roomId)>();
        foreach (var booking in bookings)
        {
            var roomsInHotel = rooms.Where(r =>
                roomCategories.Any(rc => rc.Id == r.RoomCategoryId && rc.HotelId == booking.HotelId)).ToList();

            if (!roomsInHotel.Any())
            {
                // fallback: take random rooms
                for (int i = 0; i < 1; i++)
                    bookingRoomPairs.Add((booking.Id, rooms[fakerIndex(rooms.Count)].Id));
                continue;
            }

            var take = RandomNumber(1, Math.Min(3, roomsInHotel.Count));
            var selected = roomsInHotel.OrderBy(_ => Guid.NewGuid()).Take(take).ToList();
            foreach (var r in selected)
                bookingRoomPairs.Add((booking.Id, r.Id));
        }

        // --- 12. Wire up navigation properties before saving (optional but helpful) ---
        // Add amenities to categories (1-5 per category)
        foreach (var rc in roomCategories)
        {
            var count = RandomNumber(1, 5);
            var chosen = amenities.OrderBy(_ => Guid.NewGuid()).Take(count).ToList();
            foreach (var a in chosen)
                rc.Amenities.Add(a);
        }

        // assign rooms to categories already done via Room.RoomCategoryId
        // attach categories to hotels - already via RoomCategory.HotelId

        // --- 13. Persist everything in a single transaction ---
        using var tx = await db.Database.BeginTransactionAsync(ct);

        // Insert in order with relationships:
        await db.Cities.AddRangeAsync(cities, ct);
        await db.Owners.AddRangeAsync(owners, ct);
        await db.Amenities.AddRangeAsync(amenities, ct);
        await db.Hotels.AddRangeAsync(hotels, ct);
        await db.RoomCategories.AddRangeAsync(roomCategories, ct);
        await db.Rooms.AddRangeAsync(rooms, ct);
        await db.Users.AddRangeAsync(users, ct);
        await db.Discounts.AddRangeAsync(discounts, ct);
        await db.Reviews.AddRangeAsync(reviews, ct);
        await db.Bookings.AddRangeAsync(bookings, ct);
        await db.PaymentDetails.AddRangeAsync(payments, ct);

        // Save to ensure DB keys and navigation scaffolding
        await db.SaveChangesAsync(ct);

        // Many-to-many Booking <-> Room: insert join rows
        // Using raw insert into join table for performance / determinism
        // Determine the join table name and column names based on EF conventions or your configuration.
        // Common EF Core convention for many-to-many: BookingRoom (BookingsId, RoomsId) - adjust if different.
        var joinTableName = "BookingRoom"; // adjust to actual table name if you configured it
                                           // Use SQL bulk insert via context for the pairs
        foreach (var pair in bookingRoomPairs)
        {
            // Using raw SQL insert for join table
            var sql = $"INSERT INTO [{joinTableName}]([BookingId],[RoomId]) VALUES (@p0, @p1)";
            await db.Database.ExecuteSqlRawAsync(sql, new object[] { pair.bookingId, pair.roomId }, ct);
        }

        await db.SaveChangesAsync(ct);
        await tx.CommitAsync(ct);
    }

    // Helper methods
    private static int fakerIndex(int count) => RandomNumber(0, Math.Max(0, count - 1));
    private static int RandomNumber(int min, int max) => Random.Shared.Next(min, max + 1);
    private static T RandomEnum<T>() where T : Enum
    {
        var values = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        return values[RandomNumber(0, values.Length - 1)];
    }
}