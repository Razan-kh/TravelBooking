using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.Services.Implementation;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;

namespace TravelBooking.Tests.FeaturedDeals;

public class HomeService_FeaturedDeals_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly Mock<IFeaturedHotelMapper> _mapperMock;
    private readonly HomeService _sut;

    public HomeService_FeaturedDeals_Tests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _repoMock = _fixture.Freeze<Mock<IHotelRepository>>();
        var recentlyMapper = _fixture.Freeze<Mock<IRecentlyVisitedHotelMapper>>();
        var trendingMapper = _fixture.Freeze<Mock<ITrendingCityMapper>>();

        _mapperMock = _fixture.Freeze<Mock<IFeaturedHotelMapper>>();

        _sut = new HomeService(
            _repoMock.Object,
            _mapperMock.Object,
            recentlyMapper.Object,
            trendingMapper.Object
        );
    }

    [Fact]
    public async Task GetFeaturedDealsAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var hotel =
        _fixture.Build<Hotel>()
        .Without(h => h.City)
        .Without(h => h.Owner)
        .Without(h => h.RoomCategories)
        .Without(h => h.Reviews)
        .Without(h => h.Gallery)
        .Without(h => h.Bookings)
        .Create();

        var entities = Enumerable.Range(0, 3)
        .Select(_ => _fixture.Build<HotelWithMinPrice>()
            .With(h => h.Hotel, hotel)
            .Create())
        .ToList();

        var mapped = _fixture.CreateMany<FeaturedHotelDto>(3).ToList();

        _repoMock.Setup(r => r.GetFeaturedHotelsAsync(3))
                 .ReturnsAsync(entities);

        _mapperMock.Setup(m => m.ToFeaturedHotelDto(It.IsAny<HotelWithMinPrice>()))
                   .Returns(() => _fixture.Create<FeaturedHotelDto>());

        // Act
        var result = await _sut.GetFeaturedDealsAsync(3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        _repoMock.Verify(r => r.GetFeaturedHotelsAsync(3), Times.Once);
        _mapperMock.Verify(m => m.ToFeaturedHotelDto(It.IsAny<HotelWithMinPrice>()), Times.Exactly(3));
    }

    [Fact]
    public async Task GetFeaturedDealsAsync_ShouldReturnEmpty_WhenRepositoryReturnsEmpty()
    {
        // Arrange
        _repoMock.Setup(r => r.GetFeaturedHotelsAsync(5))
                 .ReturnsAsync(new List<HotelWithMinPrice>());

        // Act
        var result = await _sut.GetFeaturedDealsAsync(5);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetFeaturedDealsAsync_ShouldPropagateExceptions()
    {
        // Arrange
        _repoMock.Setup(r => r.GetFeaturedHotelsAsync(It.IsAny<int>()))
                 .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act
        Func<Task> act = async () => await _sut.GetFeaturedDealsAsync(5);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("DB error");
    }
}