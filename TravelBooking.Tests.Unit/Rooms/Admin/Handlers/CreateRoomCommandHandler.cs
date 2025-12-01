using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Commands;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.Rooms.Admin.Handlers;

public class GetRoomCommandHandler
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomService> _serviceMock;

    public GetRoomCommandHandler()
    {
        _fixture = FixtureFactory.Create();
        _serviceMock = new Mock<IRoomService>();
    }

    [Fact]
    public async Task CreateRoomCommandHandler_Handle_ShouldReturnGuidAndCallService()
    {
        // Arrange
        var dto = _fixture.Create<CreateRoomDto>();
        var expectedId = Guid.NewGuid();
        _serviceMock.Setup(s => s.CreateRoomAsync(dto, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedId);

        var handler = new TravelBooking.Application.Rooms.Handlers.CreateRoomCommandHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new CreateRoomCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        ((Result<Guid>)result).Value.Should().Be(expectedId);
        _serviceMock.Verify(s => s.CreateRoomAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }
}