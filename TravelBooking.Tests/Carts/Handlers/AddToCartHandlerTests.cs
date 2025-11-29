using AutoFixture;
using Moq;
using TravelBooking.Application.Carts.Handlers;
using TravelBooking.Application.Carts.Commands;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Tests.Carts.TestHelpers;
using TravelBooking.Application.Shared.Results;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace TravelBooking.Tests.Carts.Handlers;

public class AddToCartHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly AddToCartHandler _handler;

    public AddToCartHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization
        {
            ConfigureMembers = false,
        });
        _fixture = FixtureFactory.Create();
        _cartServiceMock = _fixture.Freeze<Mock<ICartService>>();
        _handler = new AddToCartHandler(_cartServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenCheckOutIsBeforeCheckIn()
    {
        // Arrange
        var cmd = new AddRoomToCartCommand(
            Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today), // invalid
            1);

        // Act
        var result = await _handler.Handle(cmd, default);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal("Check-out date must be after check-in date.", result.Error);
    }

    [Fact]
    public async Task Handle_ShouldCallService_WhenRequestIsValid()
    {
        // Arrange
        var cmd = _fixture.Build<AddRoomToCartCommand>()
        .With(c => c.CheckIn, DateOnly.FromDateTime(DateTime.Today.AddDays(1)))
        .With(c => c.CheckOut, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
        .With(c => c.Quantity, 1)
        .Create();

        _cartServiceMock
            .Setup(x => x.AddRoomToCartAsync(
                cmd.UserId, cmd.RoomCategoryId, cmd.CheckIn, cmd.CheckOut, cmd.Quantity, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _handler.Handle(cmd, default);

        // Assert
        Assert.True(result.IsSuccess);

        _cartServiceMock.Verify(x =>
            x.AddRoomToCartAsync(cmd.UserId, cmd.RoomCategoryId, cmd.CheckIn, cmd.CheckOut, cmd.Quantity, It.IsAny<CancellationToken>()),
            Times.Once);
    }
}