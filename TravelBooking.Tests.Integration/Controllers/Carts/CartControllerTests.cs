using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Application.Carts.Commands;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Extensions;
using TravelBooking.Tests.Integration.Factories;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;

namespace TravelBooking.Tests.Integration.Controllers;

public class CartControllerTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private readonly DbContextOptions<AppDbContext> _dbContextOptions;

    private AppDbContext _dbContext;
    private readonly Fixture _fixture;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _role = "User";
    private readonly Guid _userId = Guid.NewGuid();
    private HttpClient _client;

    public CartControllerTests(ApiTestFactory factory)
    {
        _fixture = new Fixture();
        _factory = factory;
        _client = _factory.CreateClient();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        var scope = factory.Services.CreateScope();
        _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.EnsureDeleted();
    }

    [Fact]
    public async Task AddRoomToCart_ValidRequest_ReturnsOk()
    {
        // Arrange
        var testUserId = Guid.NewGuid();
        var roomCategory = _fixture.CreateRoomCategoryMinimal();

        var room = _fixture.CreateRoomMinimal(roomCategory);
        roomCategory.Rooms.Add(room);
        _dbContext.RoomCategories.Add(roomCategory);

        await _dbContext.SaveChangesAsync();
        
        var command = new AddRoomToCartCommand(
            roomCategory.Id,
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
            1);

        _client.AddAuthHeader(_role, testUserId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/cart", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify cart item is persisted - include Items and use proper query
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .AsNoTracking() // Important: include related data
            .FirstOrDefaultAsync(c => c.UserId == testUserId); // Use FirstOrDefaultAsync instead of FindAsync

        cart.Should().NotBeNull();
        cart!.Items.Should().ContainSingle(i =>
            i.RoomCategoryId == roomCategory.Id &&
            i.Quantity == 1);
    }

    [Fact]
    public async Task AddRoomToCart_CheckOutBeforeCheckIn_ReturnsBadRequest()
    {
        // Arrange
        var testUserId = Guid.NewGuid();

        var command = new AddRoomToCartCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), // CheckIn later
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // CheckOut earlier - invalid!
            2
        );

        _client.AddAuthHeader(_role, testUserId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/cart", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCart_WithItems_ReturnsOkWithCartItems()
    {
        // Arrange
        var testUserId = Guid.NewGuid();
        var roomCategory = _fixture.CreateRoomCategoryMinimal();
        _dbContext.RoomCategories.Add(roomCategory);
        await _dbContext.SaveChangesAsync();

        var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)); // CheckIn later
        var checkOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

        var cartItems = _fixture.Build<CartItem>()
                    .With(x => x.Quantity, 3)
                    .With(x => x.RoomCategory, roomCategory)
                    .With(x => x.CheckIn, checkIn)
                    .With(x => x.CheckOut, checkOut)
                    .Without(x => x.Cart)
                    .With(x => x.RoomCategoryId, roomCategory.Id)
                    .Create();
        cartItems.RoomCategoryId = roomCategory.Id;
        cartItems.RoomCategory = roomCategory;

        // Seed cart
        var cart = new Cart
        {
            UserId = testUserId,
            Items =
    [
        cartItems
    ]
        };
        cartItems.Cart = cart;
        cartItems.CartId = cart.Id;
        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        _client.AddAuthHeader(_role, testUserId);

        // Act
        var response = await _client.GetAsync("/api/cart");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await response.Content.ReadAsStringAsync();

        // Use an anonymous type to read the JSON
        var template = new
        {
            isSuccess = false,
            error = "",
            errorCode = "",
            httpStatusCode = (int?)null,
            Value = new List<CartItemDto>()
        };

        var result = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(json, template); result.Should().NotBeNull();
        result!.Value.Should().HaveCount(1);
    }

    [Fact]
    public async Task RemoveItem_ExistingCartItem_ReturnsOk()
    {
        // Arrange
        var testUserId = Guid.NewGuid();
        var cartItemId = Guid.NewGuid();
        var cartId = Guid.NewGuid();
        var roomCategory = _fixture.CreateRoomCategoryMinimal();
        var cart = new Cart
        {
            Id = cartId,
            UserId = testUserId,
            Items =
    [
        new CartItem
        {
            Id = cartItemId,
            RoomCategoryId = roomCategory.Id,
            RoomCategory = roomCategory,
            CheckIn = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            CheckOut = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
            Quantity = 1,
            CartId = cartId,
        }
    ]
        };
        cart.Items[0].Cart = cart;

        _dbContext.Carts.Add(cart);
        await _dbContext.SaveChangesAsync();

        _client.AddAuthHeader(_role, testUserId);

        // Act
        var response = await _client.DeleteAsync($"/api/cart/{cartItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert
        var deletedItem = await _dbContext.CartItems
            .FirstOrDefaultAsync(c => c.Id == cartItemId);

        deletedItem.Should().BeNull();
    }
    
    [Fact]
    public async Task RemoveItem_NonExistingCartItem_ReturnsNotFound()
    {
        // Arrange
        var testUserId = Guid.NewGuid();
        var nonExistingId = Guid.NewGuid();

        _client.AddAuthHeader(_role, testUserId);

        // Act
        var response = await _client.DeleteAsync($"/api/cart/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}