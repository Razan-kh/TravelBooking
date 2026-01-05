using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using TravelBooking.Application.Carts.Commands;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Extensions;
using TravelBooking.Tests.Integration.Factories;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;

namespace TravelBooking.Tests.Integration.Controllers.Carts;

public class CartControllerTests : IClassFixture<ApiTestFactory>, IDisposable
{
    private AppDbContext _dbContext;
    private readonly Fixture _fixture;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly string _role = "User";
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
        var response = await _client.PostAsJsonAsync("/api/cart/items", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .AsNoTracking() 
            .FirstOrDefaultAsync(c => c.UserId == testUserId);

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
        var hotel = new Hotel
        {
            Id = Guid.NewGuid(),
            Name = "Test Hotel"
        };
    
        var roomCategory = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Name = "Test Room Category",
            PricePerNight = 100.00m
        };
    
        await _dbContext.Hotels.AddAsync(hotel);
        await _dbContext.RoomCategories.AddAsync(roomCategory);
        await _dbContext.SaveChangesAsync();
        
        var command = new AddRoomToCartCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), // CheckIn later
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // CheckOut earlier - invalid!
            2
        );

        _client.AddAuthHeader(_role, testUserId);

        // Act
        var response = await _client.PostAsJsonAsync("/api/cart/items", command);

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

        var template = new
        {
            isSuccess = false,
            error = "",
            errorCode = "",
            httpStatusCode = (int?)null,
            Value = new List<CartItemDto>()
        };

        var result = JsonConvert.DeserializeObject<List<CartItemDto>>(json);
        result.Should().HaveCount(1);
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
        var response = await _client.DeleteAsync($"/api/cart/items/{cartItemId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

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
        var response = await _client.DeleteAsync($"/api/cart/items/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}