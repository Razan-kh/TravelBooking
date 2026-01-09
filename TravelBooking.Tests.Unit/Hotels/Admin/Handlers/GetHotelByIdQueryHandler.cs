using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Admin.Handlers;
using TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;

namespace TravelBooking.Tests.Hotels.Handlers;

public class GetHotelByIdQueryHandlerTests
{
    private readonly Mock<IHotelService> _serviceMock;
    private readonly GetHotelByIdQueryHandler _handler;
    private readonly IFixture _fixture;

    public GetHotelByIdQueryHandlerTests()
    {
        _fixture = new Fixture();
        _serviceMock = new Mock<IHotelService>();
        _handler = new GetHotelByIdQueryHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnHotelDto_WhenHotelExists()
    {
        var id = Guid.NewGuid();
        var hotelDto = _fixture.Create<HotelDto>();
        _serviceMock.Setup(s => s.GetHotelByIdAsync(id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(hotelDto);

        var result = await _handler.Handle(new GetHotelByIdQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(hotelDto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenHotelDoesNotExist()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.GetHotelByIdAsync(id, It.IsAny<CancellationToken>()))
                    .ReturnsAsync((HotelDto?)null);

        var result = await _handler.Handle(new GetHotelByIdQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Hotel not found.");
    }
}