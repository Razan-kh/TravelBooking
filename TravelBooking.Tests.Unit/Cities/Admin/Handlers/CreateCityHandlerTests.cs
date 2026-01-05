using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Handlers;
using TravelBooking.Application.Cities.Interfaces.Servicies;

namespace TravelBooking.Tests.Cities.Admin.Handlers;

public class CreateCityHandlerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ICityService> _serviceMock;
    private readonly CreateCityHandler _handler;

    public CreateCityHandlerTests()
    {
        _fixture = (Fixture?)new Fixture().Customize(new AutoMoqCustomization());
        _serviceMock = _fixture.Freeze<Mock<ICityService>>();
        _handler = new CreateCityHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnCityDto_WhenServiceSucceeds()
    {
        // Arrange
        var dto = _fixture.Create<CreateCityDto>();
        var expectedDto = _fixture.Create<CityDto>();
        _serviceMock.Setup(s => s.CreateCityAsync(dto, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedDto);

        // Act
        var result = await _handler.Handle(new CreateCityCommand(dto), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);
    }

    [Fact]
    public async Task Handle_ShouldRespectCancellationToken()
    {
        var dto = _fixture.Create<CreateCityDto>();
        var cts = new CancellationTokenSource();
        await _handler.Handle(new CreateCityCommand(dto), cts.Token);
        _serviceMock.Verify(s => s.CreateCityAsync(dto, cts.Token), Times.Once);
    }
}