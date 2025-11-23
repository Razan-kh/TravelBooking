using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Handlers;
using TravelBooking.Application.Hotels.Queries;
using TravelBooking.Application.Hotels.Servicies;

namespace TravelBooking.Tests.Hotels.Handlers;

public class GetHotelsQueryHandlerTests
{
    private readonly Mock<IHotelService> _serviceMock;
    private readonly GetHotelsQueryHandler _handler;
    private readonly IFixture _fixture;

    public GetHotelsQueryHandlerTests()
    {
        _fixture = new Fixture();
        _serviceMock = new Mock<IHotelService>();
        _handler = new GetHotelsQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnListOfHotelDto()
    {
        var hotels = _fixture.CreateMany<HotelDto>(5).ToList();
        _serviceMock.Setup(s => s.GetHotelsAsync(null, 1, 20, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(hotels);

        var result = await _handler.Handle(new GetHotelsQuery(null, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(5);
    }
}