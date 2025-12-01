using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Rooms.Admin.Services.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Tests.Rooms;

namespace TravelBooking.Tests.Rooms.Admin.Handlers;

public class GetRoomsQueryHandler
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomService> _serviceMock;

    public GetRoomsQueryHandler()
    {
        _fixture = FixtureFactory.Create();
        _serviceMock = new Mock<IRoomService>();
    }

    [Fact]
    public async Task GetRoomsQueryHandler_Handle_ShouldReturnPagedResult()
    {
        // Arrange
        var dtos = _fixture.CreateMany<RoomDto>(5).ToList();
        _serviceMock.Setup(s => s.GetRoomsAsync(null, 1, 20, It.IsAny<CancellationToken>())).ReturnsAsync(dtos);
        var handler = new TravelBooking.Application.Rooms.Handlers.GetRoomsQueryHandler(_serviceMock.Object);

        // Act
        var result = await handler.Handle(new GetRoomsQuery(null, 1, 20), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var paged = ((Result<PagedResult<RoomDto>>)result).Value;
        paged.Should().NotBeNull();
        paged!.Data.Should().HaveCount(5);
        paged.Data.Should().BeEquivalentTo(dtos);
    }
}