using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Rooms.Services.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Tests.Rooms;

namespace TravelBooking.Tests.Rooms.Admin.Handlers;
public class DeleteRoomCommandHandler
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomService> _serviceMock;

    public DeleteRoomCommandHandler()
    {
        _fixture = FixtureFactory.Create();
        _serviceMock = new Mock<IRoomService>();
    }

    [Fact]
    public async Task DeleteRoomCommandHandler_Handle_ShouldCallServiceDelete()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteRoomAsync(id, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        var handler = new TravelBooking.Application.Rooms.Handlers.DeleteRoomCommandHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new DeleteRoomCommand(id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteRoomAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}