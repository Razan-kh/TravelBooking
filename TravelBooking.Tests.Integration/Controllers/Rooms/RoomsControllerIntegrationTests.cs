using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Extensions;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using TravelBooking.Tests.Integration.Factories;

namespace TravelBooking.Tests.Integration.Controllers;

public class RoomsControllerIntegrationTests : IClassFixture<ApiTestFactory>
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly string _role = "Admin";
    private readonly Guid _adminId = Guid.NewGuid();

    public RoomsControllerIntegrationTests(ApiTestFactory factory)
    {
        _factory = factory;
        _factory.SetInMemoryDbName($"RoomsControllerTests_{Guid.NewGuid():N}");
        _client = _factory.CreateClient();
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Don't populate nav props that cause cycles
        _fixture.Customize<Room>(c => c.Without(r => r.Gallery).Without(r => r.RoomCategory).Without(r => r.Bookings));
    }

    // GET /api/rooms -> should return list
    [Fact]
    public async Task GetRooms_AdminAuthenticated_ReturnsOkWithData()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roomCategory = _fixture.CreateRoomCategoryMinimal();
        var galleryImage = _fixture.CreateGalleryMinimal();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomNumber, "101A")
            .Without(r => r.Bookings)
            .With(r => r.RoomCategory, roomCategory)
            .With(r => r.Gallery, new List<GalleryImage> { galleryImage })
            .Create();

        await db.Rooms.AddAsync(room);
        await db.SaveChangesAsync();

        // Act
        var resp = await _client.GetAsync("/api/rooms");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var payload = await resp.Content.ReadAsStringAsync();
        payload.Should().Contain("101A");
    }

    [Fact]
    public async Task GetRooms_WithoutAuth_ReturnsUnauthorized()
    {
        // Arrange - no auth header

        // Act
        var resp = await _client.GetAsync("/api/rooms");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // GET /api/rooms/{id} - not found
    [Fact]
    public async Task GetRoomById_NotFound_ReturnsNotFound()
    { 
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        var id = Guid.NewGuid();

        // Act
        var resp = await _client.GetAsync($"/api/rooms/{id}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetRoomById_ExistingId_ReturnsOk()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);

        var db = _factory.Services.GetRequiredService<AppDbContext>();

        var roomCategory = _fixture.Build<RoomCategory>()
        .Without(r => r.Amenities)
        .Without(r => r.Hotel)
        .Without(r => r.Discounts)
        .Without(r => r.Rooms)
        .Create();

        db.RoomCategories.Add(roomCategory);
        await db.SaveChangesAsync();

        var room = _fixture.Build<Room>().With(r => r.RoomCategory, roomCategory)
                                        .Without(r => r.Bookings)
                                        .Without(r => r.Gallery)
                                        .Create();
        await db.Rooms.AddAsync(room);
        await db.SaveChangesAsync();

        // Act
        var resp = await _client.GetAsync($"/api/rooms/{room.Id}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var content = await resp.Content.ReadAsStringAsync();
        content.Should().Contain(room.RoomNumber);
    }

    // POST -> CreateRoom
    [Fact]
    public async Task CreateRoom_AdminAuthenticated_ReturnsOkAndPersists()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Create a minimal CreateRoomDto (adjust shape to your actual DTO)
        var createDto = new CreateRoomDto("201", Guid.NewGuid(), true);

        var resp = await _client.PostAsJsonAsync("/api/rooms", 
            new CreateRoomCommand(createDto)
        );

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().NotBeNullOrEmpty();

        // Optionally verify DB contains a room with RoomNumber "201"
        var exists = await db.Rooms.AnyAsync(r => r.RoomNumber == "201");
        exists.Should().BeTrue();
    }

    // PUT -> UpdateRoom
    [Fact]
    public async Task UpdateRoom_AdminAuthenticated_ReturnsNoContentAndUpdates()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var roomCategory = _fixture.Build<RoomCategory>()
            .Without(r => r.Amenities)
            .Without(r => r.Hotel)
            .Without(r => r.Discounts)
            .Without(r => r.Rooms)
            .Create();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomNumber, "OldNumber")
            .With(r => r.RoomCategory, roomCategory)
            .Without(r => r.Bookings)
            .Without(r => r.Gallery)
            .Create();

        var updateCommand = new
        {
            Id = room.Id,
            Dto = new
            {
                Id = room.Id,
                RoomNumber = "UpdatedNumber",
                RoomCategoryId = roomCategory.Id,
                IsAvailable = true
            }
        };
            
        db.Rooms.Add(room);
        await db.SaveChangesAsync();
        // Act

        var resp = await _client.PutAsJsonAsync($"/api/rooms/{room.Id}", updateCommand);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Make sure DB has updated value (use AsNoTracking to avoid tracker issues)
        var updated = await db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == room.Id);
        updated!.RoomNumber.Should().Be("UpdatedNumber");
    }

    // DELETE -> DeleteRoom
    [Fact]
    public async Task DeleteRoom_AdminAuthenticated_ReturnsNoContentAndRemoves()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roomCategory = _fixture.Build<RoomCategory>()
        .Without(r => r.Amenities)
        .Without(r => r.Hotel)
        .Without(r => r.Discounts)
        .Without(r => r.Rooms)
        .Create();

        db.RoomCategories.Add(roomCategory);
        await db.SaveChangesAsync();

        var room = _fixture.Build<Room>().With(r => r.RoomCategory, roomCategory)
                                        .Without(r => r.Bookings)
                                        .Without(r => r.Gallery)
                                        .Create();


        db.Rooms.Add(room);
        await db.SaveChangesAsync();

        // Act
        var resp = await _client.DeleteAsync($"/api/rooms/{room.Id}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var exists = await db.Rooms
            .AsNoTracking()
            .AnyAsync(r => r.Id == room.Id);

        exists.Should().BeFalse();
    }
}