using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Cities.Mappers.Interfaces;

namespace TravelBooking.Tests.TrendingCities;

public class CityService_TrendingCities_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICityRepository> _repoMock;
    private readonly Mock<ITrendingCityMapper> _trendingCityMapperMock;
    private readonly Mock<ICityMapper> _cityMapperMock;
    private readonly CityService _sut;

    public CityService_TrendingCities_Tests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _repoMock = _fixture.Freeze<Mock<ICityRepository>>();
        _cityMapperMock = _fixture.Freeze<Mock<ICityMapper>>();
        _trendingCityMapperMock = _fixture.Freeze<Mock<ITrendingCityMapper>>();

        _sut = new CityService(
            _repoMock.Object,
            _cityMapperMock.Object,
            _trendingCityMapperMock.Object
        );
    }

    [Fact]
    public async Task GetTrendingCitiesAsync_ShouldReturnMappedResults_WhenSuccess  ()
    {
        _fixture.Customize<City>(c => c
            .With(x => x.Id, Guid.NewGuid())
            .Without(x => x.Hotels)
        );
        var city = _fixture.Create<City>();

        _repoMock
            .Setup(r => r.GetTrendingCitiesAsync(3))
            .ReturnsAsync(new List<(City city, int visitCount)>
            {
                (_fixture.Build<City>().Without(c => c.Hotels).Create(), 120),
                (_fixture.Build<City>().Without(c => c.Hotels).Create(), 90),
                (_fixture.Build<City>().Without(c => c.Hotels).Create(), 60),
            });

        _trendingCityMapperMock
        .Setup(m => m.ToTrendingCityDto(It.IsAny<(City city, int visitCount)>()))
        .Returns(() => _fixture.Create<TrendingCityDto>());

        var result = await _sut.GetTrendingCitiesAsync(3);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetTrendingCitiesAsync_ShouldReturnEmpty_WhenRepositoryReturnsEmpty()
    {
        _repoMock
            .Setup(r => r.GetTrendingCitiesAsync(3))
            .ReturnsAsync(new List<(City city, int visitCount)>
            {});

        var result = await _sut.GetTrendingCitiesAsync(3);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetTrendingCitiesAsync_ShouldPropagateException()
    {
        _repoMock.Setup(r => r.GetTrendingCitiesAsync(It.IsAny<int>()))
                 .ThrowsAsync(new Exception("DB error"));

        Func<Task> act = () => _sut.GetTrendingCitiesAsync(4);

        await act.Should().ThrowAsync<Exception>().WithMessage("DB error");
    }
}