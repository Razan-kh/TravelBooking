using TravelBooking.Application.Cities.Dtos;
using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.Handlers;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Cities.Mappers.Interfaces;
using TravelBooking.Tests.Shared;
using TravelBooking.Application.Cities.Admin.Queries;

namespace TravelBooking.Tests.Cities.Admin.Handlers;

public class GetCityByIdQueryHandlerTests
{
    private readonly Mock<ICityService> _serviceMock;
    private readonly Mock<ICityMapper> _mapperMock;
    private readonly GetCityByIdQueryHandler _handler;
    private readonly IFixture _fixture;

    public GetCityByIdQueryHandlerTests()
    {
        _serviceMock = new Mock<ICityService>();
        _fixture = new Fixture();
        _fixture.Customize(new EntityCustomization()); 
        _mapperMock = new Mock<ICityMapper>();
        _handler = new GetCityByIdQueryHandler(_serviceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnDto_WhenCityExists()
    {
        // Arrange
        var city = _fixture.Create<City>();
        var dto = new CityDto();
        _serviceMock.Setup(s => s.GetCityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync(dto);
        // Act
        var result = await _handler.Handle(new GetCityByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenCityDoesNotExist()
    {
        _serviceMock.Setup(s => s.GetCityByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                    .ReturnsAsync((CityDto?)null);

        var result = await _handler.Handle(new GetCityByIdQuery(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("City not found.");
    }
}