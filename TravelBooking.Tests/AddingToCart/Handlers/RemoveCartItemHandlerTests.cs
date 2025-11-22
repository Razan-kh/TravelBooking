using AutoFixture;
using Moq;
using TravelBooking.Application.AddingToCart.Handlers;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Tests.AddingToCart.TestHelpers;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.AddingToCart.Handlers;

public class RemoveCartItemHandlerTests
{
    private readonly Mock<ICartService> _serviceMock;
    private readonly RemoveCartItemHandler _handler;

    public RemoveCartItemHandlerTests()
    {
        var fixture = FixtureFactory.Create();
        _serviceMock = fixture.Freeze<Mock<ICartService>>();
        _handler = new RemoveCartItemHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallService()
    {
        // Arrange
        var id = Guid.NewGuid();
        var cmd = new RemoveCartItemCommand(id);

        _serviceMock
            .Setup(x => x.RemoveItemAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(cmd, default);

        // Assert
        Assert.True(result.IsSuccess);

        _serviceMock.Verify(x => x.RemoveItemAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}