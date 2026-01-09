using AutoFixture;
using Moq;
using TravelBooking.Application.Carts.Handlers;
using TravelBooking.Application.Carts.Queries;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Tests.Carts.TestHelpers;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.Carts.Handlers;

public class GetCartQueryHandlerTests
{
    private readonly Mock<ICartService> _cartService;
    private readonly GetCartQueryHandler _handler;

    public GetCartQueryHandlerTests()
    {
        var fixture = FixtureFactory.Create();
        _cartService = fixture.Freeze<Mock<ICartService>>();
        _handler = new GetCartQueryHandler(_cartService.Object);
    }

    [Fact]
    public async Task Handle_ServiceReturnsCartItems_ReturnsCartItems()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var items = new List<CartItemDto>();
        var expected = Result.Success(items);

        _cartService
            .Setup(x => x.GetCartAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var query = new GetCartQuery
        {
            UserId = userId
        };

        // Act
        var result = await _handler.Handle(query, default);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(items, result.Value);
    }
}