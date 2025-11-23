using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.Services.Implementation;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;

namespace TravelBooking.Tests.TrendingCities;

public class HomeService_TrendingCities_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly Mock<ITrendingCityMapper> _mapperMock;
    private readonly HomeService _sut;

    public HomeService_TrendingCities_Tests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _fixture.Behaviors
            .OfType<ThrowingRecursionBehavior>()
            .ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));

        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _repoMock = _fixture.Freeze<Mock<IHotelRepository>>();
        var featuredMapper = _fixture.Freeze<Mock<IFeaturedHotelMapper>>();
        var recentlyMapper = _fixture.Freeze<Mock<IRecentlyVisitedHotelMapper>>();
        _mapperMock = _fixture.Freeze<Mock<ITrendingCityMapper>>();

        _sut = new HomeService(
            _repoMock.Object,
            featuredMapper.Object,
            recentlyMapper.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task GetTrendingCitiesAsync_ShouldReturnMappedResults()
    {
        var cities = _fixture.Build<City>()
            .Without(c => c.Hotels)
            .CreateMany(3)
            .ToList();

        _repoMock
            .Setup(r => r.GetTrendingCitiesAsync(3))
            .ReturnsAsync(new List<(City city, int visitCount)>
            {
                (_fixture.Build<City>().Without(c => c.Hotels).Create(), 120),
                (_fixture.Build<City>().Without(c => c.Hotels).Create(), 90),
                (_fixture.Build<City>().Without(c => c.Hotels).Create(), 60),
            });

        _mapperMock
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