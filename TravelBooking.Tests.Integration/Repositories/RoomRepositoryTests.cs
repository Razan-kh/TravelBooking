using System.Linq;
using System.Threading;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using Xunit;
using AutoFixture.Kernel;
using TravelBooking.Domain.Owners.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Tests.Integration.Extensions;

namespace TravelBooking.Tests.Integration.Repositories
{
    public class RoomRepositoryTests
    {
        private readonly ApiTestFactory _factory;
        private readonly IFixture _fixture;

        public RoomRepositoryTests()
        {
            _factory = new ApiTestFactory(); // unique DB per instance
            _fixture = new Fixture();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(_fixture.Behaviors.OfType<ThrowingRecursionBehavior>().Single());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        }

        private AppDbContext CreateContext()
        {
            var scope = _factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        [Fact]
        public async Task GetRoomsAsync_Filter_ReturnsExpectedRooms()
        {
            // Arrange
            var db = CreateContext();
            var repo = new RoomRepository(db); // adjust constructor/namespace as needed

            var roomCategory = _fixture.CreateRoomCategoryMinimal();
            var galleryImage = _fixture.CreateGalleryMinimal();
            var matching = _fixture.Build<Room>()
                .With(r => r.RoomNumber, "Match-100")
                .With(r => r.RoomCategory, roomCategory)
                .With(r => r.Gallery, new List<GalleryImage> { galleryImage })
                .Without(r => r.Bookings)
                .Create();

            var nonMatching = _fixture.Build<Room>()
                .With(r => r.RoomNumber, "Other")
                .With(r => r.RoomCategory, roomCategory)
                .With(r => r.Gallery, new List<GalleryImage> { galleryImage })
                .Without(r => r.Bookings)
                .Create();

            await db.Rooms.AddRangeAsync(matching, nonMatching);
            await db.SaveChangesAsync();

            // Act
            var result = await repo.GetRoomsAsync("Match", CancellationToken.None);

            // Assert
            result.Should().ContainSingle(r => r.RoomNumber == "Match-100");
        }

        [Fact]
        public async Task GetByIdAsync_ExistingId_ReturnsRoom()
        {
            // Arrange
            var db = CreateContext();
            var repo = new RoomRepository(db);

            var roomCategory = _fixture.Build<RoomCategory>()
            .Without(r => r.Amenities)
            .Without(r => r.Hotel)
            .Without(r => r.Discounts)
            .Without(r => r.Rooms)
            .Create();

            var galleryImage = _fixture.Build<GalleryImage>()
            .Create();

            var room = _fixture.Build<Room>()
            .Without(r => r.Bookings)
                        .With(r => r.RoomCategory, roomCategory)
                        .With(r => r.Gallery, new List<GalleryImage> { galleryImage })

            .Create();
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();

            // Act
            var got = await repo.GetByIdAsync(room.Id, CancellationToken.None);

            // Assert
            got.Should().NotBeNull();
            got!.Id.Should().Be(room.Id);
        }

        [Fact]
        public async Task AddAsync_AddsRoom()
        {
            // Arrange
            var db = CreateContext();
            var repo = new RoomRepository(db);
            var roomCategory = _fixture.CreateRoomCategoryMinimal();
            var room = _fixture.CreateRoomMinimal(roomCategory);

            // Act
            await repo.AddAsync(room, CancellationToken.None);

            // Assert
            var exists = await db.Rooms.AnyAsync(r => r.Id == room.Id);
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateAsync_UpdatesRoomProperties()
        {
            // Arrange
            var db = CreateContext();
            var repo = new RoomRepository(db);

            var room = _fixture.Build<Room>().With(r => r.RoomNumber, "Before").Without(r => r.RoomCategory)
            .Without(r => r.RoomCategory)
            .Without(r => r.Bookings)
            .Without(r => r.Gallery)
            .Create();
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();

            // Modify and call update
            room.RoomNumber = "After";
            await repo.UpdateAsync(room, CancellationToken.None);

            var updated = await db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == room.Id);
            updated!.RoomNumber.Should().Be("After");
        }

        [Fact]
        public async Task DeleteAsync_RemovesRoom()
        {
            // Arrange
            var db = CreateContext();
            var repo = new RoomRepository(db);

            var room = _fixture.Build<Room>()
            .Without(r => r.RoomCategory)
            .Without(r => r.Bookings)
            .Without(r => r.Gallery)
            .Create();
            await db.Rooms.AddAsync(room);
            await db.SaveChangesAsync();

            // Act
            await repo.DeleteAsync(room, CancellationToken.None);

            // Assert
            var exists = await db.Rooms.AnyAsync(r => r.Id == room.Id);
            exists.Should().BeFalse();
        }
    }
}