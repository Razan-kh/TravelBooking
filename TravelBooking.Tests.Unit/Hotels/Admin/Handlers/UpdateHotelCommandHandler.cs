using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Admin.Handlers;
using TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

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
        _serviceMock
        .Setup(s => s.UpdateHotelAsync(dto, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.Success());


        var result = await _handler.Handle(new UpdateHotelCommand(dto), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenHotelNotFound()
    {
        // Arrange 
        var dto = _fixture.Create<UpdateHotelDto>();
        _serviceMock.Setup(s => s.UpdateHotelAsync(dto, It.IsAny<CancellationToken>()))
        .ReturnsAsync(Result.NotFound($"Hotel with ID {dto.Id} not found."));

        // Act
        var result = await _handler.Handle(new UpdateHotelCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.ErrorCode.Should().Be("NOT_FOUND");

        _serviceMock.Verify(
        s => s.UpdateHotelAsync(dto, It.IsAny<CancellationToken>()),
        Times.Once
    );
    }
}