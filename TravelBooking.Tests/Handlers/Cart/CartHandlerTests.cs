using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using TravelBooking.Application.AddingToCart.Handlers;
using TravelBooking.Application.AddingToCart.Services.Implementations;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.AddingToCart.Queries;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.AddingToCart.Mappers;
using Xunit;
/*
namespace TravelBooking.Tests.Handlers.Cart;

public class CartHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICartService> _cartServiceMock;

    public CartHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _cartServiceMock = _fixture.Freeze<Mock<ICartService>>();
    }

    [Fact]
    public async Task AddToCartHandler_ShouldReturnFailure_WhenCheckOutBeforeCheckIn()
    {
        // Arrange
        var handler = new AddToCartHandler(_cartServiceMock.Object);
        var command = new AddRoomToCartCommand
        {
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Check-out date must be after check-in date.", result.Message);
    }

    [Fact]
    public async Task AddToCartHandler_ShouldCallService_WhenDatesValid()
    {
        // Arrange
        var handler = new AddToCartHandler(_cartServiceMock.Object);
        var command = _fixture.Create<AddRoomToCartCommand>();
        _cartServiceMock
            .Setup(x => x.AddRoomToCartAsync(command.UserId, command.RoomCategoryId,
                command.CheckIn, command.CheckOut, command.Quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _cartServiceMock.Verify(x => x.AddRoomToCartAsync(
            command.UserId, command.RoomCategoryId, command.CheckIn, command.CheckOut, command.Quantity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCartQueryHandler_ShouldReturnCartItems()
    {
        // Arrange
        var handler = new GetCartQueryHandler(_cartServiceMock.Object);
        var userId = Guid.NewGuid();
        var items = _fixture.Create<List<CartItemDto>>();

        _cartServiceMock
            .Setup(x => x.GetCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(items));

        // Act
        var result = await handler.Handle(new GetCartQuery { UserId = userId }, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(items.Count, result.Value.Count);
    }

    [Fact]
    public async Task RemoveCartItemHandler_ShouldCallService()
    {
        // Arrange
        var handler = new RemoveCartItemHandler(_cartServiceMock.Object);
        var cartItemId = Guid.NewGuid();

        _cartServiceMock
            .Setup(x => x.RemoveItemAsync(cartItemId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await handler.Handle(new RemoveCartItemCommand { CartItemId = cartItemId }, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _cartServiceMock.Verify(x => x.RemoveItemAsync(cartItemId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
*/