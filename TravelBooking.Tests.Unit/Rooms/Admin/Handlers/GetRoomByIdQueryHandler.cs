using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.Rooms.Admin.Handlers;

public class GetRoomByIdQueryHandler
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomService> _serviceMock;

    public GetRoomByIdQueryHandler()
    {
        _fixture = FixtureFactory.Create();
        _serviceMock = new Mock<IRoomService>();
    }

    [Fact]
    public async Task GetRoomByIdQueryHandler_Handle_ShouldReturnFailure_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetRoomByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((RoomDto?)null);
        var handler = new TravelBooking.Application.Rooms.Handlers.GetRoomByIdQueryHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new GetRoomByIdQuery(id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Room not found");
    }

    [Fact]
    public async Task GetRoomByIdQueryHandler_Handle_ShouldReturnSuccess_WhenFound()
    {
        // Arrange
        var roomDto = _fixture.Create<RoomDto>();
        _serviceMock.Setup(s => s.GetRoomByIdAsync(roomDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(roomDto);
        var handler = new TravelBooking.Application.Rooms.Handlers.GetRoomByIdQueryHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new GetRoomByIdQuery(roomDto.Id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ((Result<RoomDto>)result).Value.Should().BeEquivalentTo(roomDto);
    }
}