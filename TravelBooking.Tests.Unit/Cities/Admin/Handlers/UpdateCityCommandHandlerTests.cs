using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Handlers;
using TravelBooking.Application.Cities.Interfaces.Servicies;

namespace TravelBooking.Tests.Cities.Admin.Handlers;

public class UpdateCityCommandHandlerTests
{
    private readonly Mock<ICityService> _serviceMock;
    private readonly UpdateCityCommandHandler _handler;
    private readonly IFixture _fixture;

    public UpdateCityCommandHandlerTests()
    {
        _serviceMock = new Mock<ICityService>();
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _handler = new UpdateCityCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenCityUpdated()
    {
        var dto = _fixture.Create<UpdateCityDto>();
        var result = await _handler.Handle(new UpdateCityCommand(dto), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.UpdateCityAsync(dto, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCityNotFound()
    {
        var dto = _fixture.Create<UpdateCityDto>();
        _serviceMock.Setup(s => s.UpdateCityAsync(dto, It.IsAny<CancellationToken>()))
                    .ThrowsAsync(new KeyNotFoundException($"City with ID {dto.Id} not found."));

        var result = await _handler.Handle(new UpdateCityCommand(dto), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("NOT_FOUND");
    }
}