using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.Rooms.Admin.Handlers;

public class UpdateRoomCommandHandler
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomService> _serviceMock;

    public UpdateRoomCommandHandler()
    {
        _fixture = FixtureFactory.Create();
        _serviceMock = new Mock<IRoomService>();
    }

    [Fact]
    public async Task UpdateRoomCommandHandler_Handle_ShouldReturnFailure_WhenServiceReturnsNotFound()
    {
        // Arrange
        var dto = _fixture.Create<UpdateRoomDto>();
        _serviceMock.Setup(s => s.UpdateRoomAsync(dto, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.NotFound($"not found."));

        var handler = new TravelBooking.Application.Rooms.Handlers.UpdateRoomCommandHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new UpdateRoomCommand(dto.Id, dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.ErrorCode.Should().Be("NOT_FOUND");

    }

    [Fact]
    public async Task UpdateRoomCommandHandler_Handle_ShouldReturnSuccess_WhenUpdateSucceeds()
    {
        // Arrange
        var dto = _fixture.Create<UpdateRoomDto>();
        _serviceMock.Setup(s => s.UpdateRoomAsync(dto, It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());
        var handler = new TravelBooking.Application.Rooms.Handlers.UpdateRoomCommandHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new UpdateRoomCommand(dto.Id, dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateRoomAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }
}