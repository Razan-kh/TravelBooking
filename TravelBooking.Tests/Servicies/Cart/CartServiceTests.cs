using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using TravelBooking.Application.Carts.Handlers;
using TravelBooking.Application.Carts.Services.Implementations;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Carts.Mappers;

using TravelBooking.Application.Carts.Mappers;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Application.Shared.Interfaces;

using Xunit;
//not important
/* 
namespace TravelBooking.Tests.Services.Cart;

public class CartServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomAvailabilityService> _availabilityMock;
    private readonly Mock<ICartRepository> _cartRepoMock;
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<ICartMapper> _mapperMock;
    private readonly CartService _service;

    public CartServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _availabilityMock = _fixture.Freeze<Mock<IRoomAvailabilityService>>();
        _cartRepoMock = _fixture.Freeze<Mock<ICartRepository>>();
        _uowMock = _fixture.Freeze<Mock<IUnitOfWork>>();
        _mapperMock = _fixture.Freeze<Mock<ICartMapper>>();

        _service = new CartService(_availabilityMock.Object, _cartRepoMock.Object, _uowMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task AddRoomToCartAsync_ShouldFail_WhenNotEnoughRooms()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();
        _availabilityMock
            .Setup(x => x.HasAvailableRoomsAsync(roomId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _service.AddRoomToCartAsync(userId, roomId, DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Not enough rooms available for the selected period.", result.Message);
    }

    [Fact]
    public async Task AddRoomToCartAsync_ShouldAddNewItem_WhenCartEmpty()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        _availabilityMock
            .Setup(x => x.HasAvailableRoomsAsync(roomId, It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _cartRepoMock
            .Setup(x => x.GetUserCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TravelBooking.Domain.Carts.Entities.Cart)null);

        // Act
        var result = await _service.AddRoomToCartAsync(userId, roomId,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), 1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _cartRepoMock.Verify(x => x.AddOrUpdateAsync(It.IsAny<TravelBooking.Domain.Carts.Entities.Cart>()), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldFail_WhenItemNotFound()
    {
        // Arrange
        var itemId = Guid.NewGuid();
        _cartRepoMock
            .Setup(x => x.GetCartItemByIdAsync(itemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CartItem)null);

        // Act
        var result = await _service.RemoveItemAsync(itemId, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Cart item not found.", result.Message);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldRemoveItem_WhenExists()
    {
        // Arrange
        var item = _fixture.Create<CartItem>();
        _cartRepoMock
            .Setup(x => x.GetCartItemByIdAsync(item.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(item);

        // Act
        var result = await _service.RemoveItemAsync(item.Id, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _cartRepoMock.Verify(x => x.RemoveItem(item), Times.Once);
        _uowMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
*/