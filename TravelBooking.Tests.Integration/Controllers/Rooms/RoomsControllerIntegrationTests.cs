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

public class RoomsControllerIntegrationTests : IClassFixture<ApiTestFactory>, IAsyncLifetime
{
    private readonly ApiTestFactory _factory;
    private readonly HttpClient _client;
    private readonly IFixture _fixture;
    private readonly string _role = "Admin";
    private readonly Guid _adminId = Guid.NewGuid();
    private IServiceScope _scope;
    private AppDbContext _db;

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
    public async Task InitializeAsync()
    {
        _scope = _factory.Services.CreateScope();
        _db = _scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // Ensure clean database
        await _db.Database.EnsureDeletedAsync();
        await _db.Database.EnsureCreatedAsync();

        // Clear any existing data
        //    _dbContext.Rooms.RemoveRange(_dbContext.Rooms);
        //    _dbContext.RoomCategories.RemoveRange(_dbContext.RoomCategories);
        //    await _dbContext.SaveChangesAsync();
    }
    public async Task DisposeAsync()
    {
        // Cleanup
        if (_db != null)
        {
            await _db.Database.EnsureDeletedAsync();
        }

        _scope?.Dispose();
        _client?.Dispose();
       // await _factory.DisposeAsync();
    }
    
    // GET /api/rooms -> should return list
    [Fact]
    public async Task GetRooms_AdminAuthenticated_ReturnsOkWithData()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
    //    using var scope = _factory.Services.CreateScope();
    //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roomCategory = _fixture.CreateRoomCategoryMinimal();
        var galleryImage = _fixture.CreateGalleryMinimal();

        var room = _fixture.Build<Room>()
            .With(r => r.RoomNumber, "101A")
            .Without(r => r.Bookings)
            .With(r => r.RoomCategory, roomCategory)
            .With(r => r.Gallery, new List<GalleryImage> { galleryImage })
            .Create();

        await _db.Rooms.AddAsync(room);
        await _db.SaveChangesAsync();

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

    //    var db = _factory.Services.GetRequiredService<AppDbContext>();

        var roomCategory = _fixture.Build<RoomCategory>()
        .Without(r => r.Amenities)
        .Without(r => r.Hotel)
        .Without(r => r.Discounts)
        .Without(r => r.Rooms)
        .Create();

        _db.RoomCategories.Add(roomCategory);
        await _db.SaveChangesAsync();

        var room = _fixture.Build<Room>().With(r => r.RoomCategory, roomCategory)
                                        .Without(r => r.Bookings)
                                        .Without(r => r.Gallery)
                                        .Create();
        await _db.Rooms.AddAsync(room);
        await _db.SaveChangesAsync();

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
     //   using var scope = _factory.Services.CreateScope();
     //   var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
        var exists = await _db.Rooms.AnyAsync(r => r.RoomNumber == "201");
        exists.Should().BeTrue();
    }

    // PUT -> UpdateRoom
    [Fact]
    public async Task UpdateRoom_AdminAuthenticated_ReturnsNoContentAndUpdates()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
    //    using var scope = _factory.Services.CreateScope();
    //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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
            
        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();
        // Act

        var resp = await _client.PutAsJsonAsync($"/api/rooms/{room.Id}", updateCommand);

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Make sure DB has updated value (use AsNoTracking to avoid tracker issues)
        var updated = await _db.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == room.Id);
        updated!.RoomNumber.Should().Be("UpdatedNumber");
    }

    // DELETE -> DeleteRoom
    [Fact]
    public async Task DeleteRoom_AdminAuthenticated_ReturnsNoContentAndRemoves()
    {
        // Arrange
        _client.AddAuthHeader(_role, _adminId);
      //  using var scope = _factory.Services.CreateScope();
    //    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roomCategory = _fixture.Build<RoomCategory>()
        .Without(r => r.Amenities)
        .Without(r => r.Hotel)
        .Without(r => r.Discounts)
        .Without(r => r.Rooms)
        .Create();

        _db.RoomCategories.Add(roomCategory);
        await _db.SaveChangesAsync();

        var room = _fixture.Build<Room>().With(r => r.RoomCategory, roomCategory)
                                        .Without(r => r.Bookings)
                                        .Without(r => r.Gallery)
                                        .Create();


        _db.Rooms.Add(room);
        await _db.SaveChangesAsync();

        // Act
        var resp = await _client.DeleteAsync($"/api/rooms/{room.Id}");

        // Assert
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var exists = await _db.Rooms
            .AsNoTracking()
            .AnyAsync(r => r.Id == room.Id);

        exists.Should().BeFalse();
    }
}