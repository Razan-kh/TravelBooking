using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Handlers;
using TravelBooking.Application.TrendingCities.Queries;

namespace TravelBooking.Tests.TrendingCities;

public class GetTrendingCitiesHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHomeService> _homeServiceMock;
    private readonly GetTrendingCitiesHandler _sut;

    public GetTrendingCitiesHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _homeServiceMock = _fixture.Freeze<Mock<IHomeService>>();
        _sut = new GetTrendingCitiesHandler(_homeServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenServiceReturnsData()
    {
        var count = 4;
        var expected = _fixture.CreateMany<TrendingCityDto>(count).ToList();

        _homeServiceMock
            .Setup(s => s.GetTrendingCitiesAsync(count))
            .ReturnsAsync(Result.Success(expected));

        var query = new GetTrendingCitiesQuery(count);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenServiceFails()
    {
        _homeServiceMock
            .Setup(s => s.GetTrendingCitiesAsync(3))
            .ReturnsAsync(Result.Failure<List<TrendingCityDto>>("ERR"));

        var query = new GetTrendingCitiesQuery(3);

        var result = await _sut.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("ERR");
    }
}