using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Handlers;
using TravelBooking.Application.Hotels.Servicies;

namespace TravelBooking.Tests.Hotels.Handlers;

public class CreateHotelCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelService> _serviceMock;
    private readonly CreateHotelCommandHandler _handler;

    public CreateHotelCommandHandlerTests()
    {
        _fixture = new Fixture();
        _serviceMock = new Mock<IHotelService>();
        _handler = new CreateHotelCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnHotelDto_WhenServiceSucceeds()
    {
        var dto = _fixture.Create<CreateHotelDto>();
        var expectedDto = _fixture.Create<HotelDto>();

        _serviceMock.Setup(s => s.CreateHotelAsync(dto, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedDto);

        var result = await _handler.Handle(new CreateHotelCommand(dto), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
    }
}