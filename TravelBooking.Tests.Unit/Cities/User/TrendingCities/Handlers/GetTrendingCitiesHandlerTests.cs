using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Handlers;
using TravelBooking.Application.TrendingCities.Queries;
using TravelBooking.Application.Cities.Interfaces.Servicies;

namespace TravelBooking.Tests.TrendingCities;

public class GetTrendingCitiesHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICityService> _cityServiceMock;
    private readonly GetTrendingCitiesHandler _sut;

    public GetTrendingCitiesHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _cityServiceMock = _fixture.Freeze<Mock<ICityService>>();
        _sut = new GetTrendingCitiesHandler(_cityServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenServiceReturnsData()
    {
        // Arrange
        var count = 4;
        var expected = _fixture.CreateMany<TrendingCityDto>(count).ToList();

        _cityServiceMock
            .Setup(s => s.GetTrendingCitiesAsync(count))
            .ReturnsAsync(Result.Success(expected));

        var query = new GetTrendingCitiesQuery(count);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenServiceFails()
    {
        // Arrange
        _cityServiceMock
            .Setup(s => s.GetTrendingCitiesAsync(3))
            .ReturnsAsync(Result.Failure<List<TrendingCityDto>>("ERR"));

        var query = new GetTrendingCitiesQuery(3);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("ERR");
    }
}