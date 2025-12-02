using TravelBooking.Domain.Rooms.Entities;
using Xunit;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Tests.Integration.Repositories;

public class CartRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly CartRepository _repository;

    public CartRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new CartRepository(_dbContext);
    }

    public async Task InitializeAsync()
    {
        // Seed test data
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        _dbContext?.Dispose();
        return Task.CompletedTask;
    }

    private async Task SeedTestDataAsync()
    {
        var userId = Guid.NewGuid();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Items = new List<CartItem>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        RoomCategoryId = Guid.NewGuid(),
                        Quantity = 2,
                        RoomCategory = new RoomCategory { Id = Guid.NewGuid(), Name = "Standard Room" }
                    }
                }
        };

        await _dbContext.Carts.AddAsync(cart);
        await _dbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetUserCartAsync_ExistingUser_ReturnsCartWithItems()
    {
        // Arrange
        var existingUserId = _dbContext.Carts.First().UserId;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetUserCartAsync(existingUserId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.UserId.Should().Be(existingUserId);
        result.Items.Should().HaveCount(1);
        result.Items.First().RoomCategory.Should().NotBeNull();
    }

    [Fact]
    public async Task GetUserCartAsync_NonExistingUser_ReturnsNull()
    {
        // Arrange
        var nonExistingUserId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetUserCartAsync(nonExistingUserId, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetCartItemByIdAsync_ExistingCartItem_ReturnsCartItem()
    {
        // Arrange
        var existingCartItemId = _dbContext.CartItems.First().Id;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetCartItemByIdAsync(existingCartItemId, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingCartItemId);
    }

    [Fact]
    public async Task GetCartItemByIdAsync_NonExistingCartItem_ReturnsNull()
    {
        // Arrange
        var nonExistingCartItemId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetCartItemByIdAsync(nonExistingCartItemId, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task RemoveItem_ExistingCartItem_RemovesFromDatabase()
    {
        // Arrange
        var existingCartItem = _dbContext.CartItems.First();
        var initialCount = await _dbContext.CartItems.CountAsync();

        // Act
        _repository.RemoveItem(existingCartItem);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCount = await _dbContext.CartItems.CountAsync();
        finalCount.Should().Be(initialCount - 1);
        _dbContext.CartItems.Find(existingCartItem.Id).Should().BeNull();
    }

    [Fact]
    public async Task ClearUserCartAsync_ExistingUser_RemovesAllCartItems()
    {
        // Arrange
        var existingUserId = _dbContext.Carts.First().UserId;
        var initialItemCount = _dbContext.CartItems.Count(ci => ci.Cart.UserId == existingUserId);

        // Act
        await _repository.ClearUserCartAsync(existingUserId, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalItemCount = _dbContext.CartItems.Count(ci => ci.Cart.UserId == existingUserId);
        finalItemCount.Should().Be(0);
    }

    [Fact]
    public async Task ClearUserCartAsync_NonExistingUser_DoesNothing()
    {
        // Arrange
        var nonExistingUserId = Guid.NewGuid();
        var initialCartCount = await _dbContext.Carts.CountAsync();

        // Act
        await _repository.ClearUserCartAsync(nonExistingUserId, CancellationToken.None);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCartCount = await _dbContext.Carts.CountAsync();
        finalCartCount.Should().Be(initialCartCount);
    }

    [Fact]
    public async Task AddOneAsync_NewCart_AddsToDatabase()
    {
        // Arrange
        var newCart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Items = new List<CartItem>()
        };
        var initialCount = await _dbContext.Carts.CountAsync();

        // Act
        await _repository.AddOneAsync(newCart);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCount = await _dbContext.Carts.CountAsync();
        finalCount.Should().Be(initialCount + 1);
        _dbContext.Carts.Find(newCart.Id).Should().NotBeNull();
    }

    [Fact]
    public async Task AddOrUpdateAsync_NewCart_AddsCartToDatabase()
    {
        // Arrange
        var newCart = new Cart
        {
            Id = Guid.Empty, // Indicates new cart
            UserId = Guid.NewGuid(),
            Items = new List<CartItem>()
        };
        var initialCount = await _dbContext.Carts.CountAsync();

        // Act
        await _repository.AddOrUpdateAsync(newCart);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCount = await _dbContext.Carts.CountAsync();
        finalCount.Should().Be(initialCount + 1);
        newCart.Id.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public async Task AddOrUpdateAsync_ExistingCart_UpdatesCartInDatabase()
    {
        // Arrange
        var existingCart = _dbContext.Carts.First();
        var updatedUserId = Guid.NewGuid();
        existingCart.UserId = updatedUserId;

        // Act
        await _repository.AddOrUpdateAsync(existingCart);
        await _dbContext.SaveChangesAsync();

        // Assert
        var updatedCart = _dbContext.Carts.Find(existingCart.Id);
        updatedCart.Should().NotBeNull();
        updatedCart!.UserId.Should().Be(updatedUserId);
    }

    [Fact]
    public async Task ClearCartAsync_ExistingUser_ClearsAllCartItems()
    {
        // Arrange
        var existingUserId = _dbContext.Carts.First().UserId;
        var cart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstAsync(c => c.UserId == existingUserId);
        var initialItemCount = cart.Items.Count;

        // Act
        await _repository.ClearCartAsync(existingUserId, CancellationToken.None);

        // Assert
        var updatedCart = await _dbContext.Carts
            .Include(c => c.Items)
            .FirstAsync(c => c.UserId == existingUserId);
        updatedCart.Items.Should().BeEmpty();
        initialItemCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ClearCartAsync_NonExistingUser_DoesNothing()
    {
        // Arrange
        var nonExistingUserId = Guid.NewGuid();
        var initialCartCount = await _dbContext.Carts.CountAsync();

        // Act
        await _repository.ClearCartAsync(nonExistingUserId, CancellationToken.None);

        // Assert
        var finalCartCount = await _dbContext.Carts.CountAsync();
        finalCartCount.Should().Be(initialCartCount);
    }
}