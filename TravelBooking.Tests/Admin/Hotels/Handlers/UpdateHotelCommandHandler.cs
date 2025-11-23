using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Handlers;
using TravelBooking.Application.Hotels.Servicies;

namespace TravelBooking.Tests.Hotels.Handlers;

public class UpdateHotelCommandHandlerTests
{
    private readonly Mock<IHotelService> _serviceMock;
    private readonly UpdateHotelCommandHandler _handler;
    private readonly IFixture _fixture;

    public UpdateHotelCommandHandlerTests()
    {
        _fixture = new Fixture();
        _serviceMock = new Mock<IHotelService>();
        _handler = new UpdateHotelCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenUpdateSucceeds()
    {
        var dto = _fixture.Create<UpdateHotelDto>();
        _serviceMock.Setup(s => s.UpdateHotelAsync(dto, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new UpdateHotelCommand(dto), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenHotelNotFound()
    {
        var dto = _fixture.Create<UpdateHotelDto>();
        _serviceMock.Setup(s => s.UpdateHotelAsync(dto, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new KeyNotFoundException("Hotel not found"));

        var result = await _handler.Handle(new UpdateHotelCommand(dto), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Hotel not found");
    }
}