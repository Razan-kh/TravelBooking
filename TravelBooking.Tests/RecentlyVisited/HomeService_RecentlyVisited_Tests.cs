using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.Services.Implementation;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Application.RecentlyVisited.Dtos;

namespace TravelBooking.Tests.Application.RecentlyVisited;

public class HomeService_RecentlyVisited_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly Mock<IRecentlyVisitedHotelMapper> _mapperMock;
    private readonly HomeService _sut;

    public HomeService_RecentlyVisited_Tests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _repoMock = _fixture.Freeze<Mock<IHotelRepository>>();
        var featuredMapper = _fixture.Freeze<Mock<IFeaturedHotelMapper>>();
        var trendingMapper = _fixture.Freeze<Mock<ITrendingCityMapper>>();
        _mapperMock = _fixture.Freeze<Mock<IRecentlyVisitedHotelMapper>>();

        _sut = new HomeService(
            _repoMock.Object,
            featuredMapper.Object,
            _mapperMock.Object,
            trendingMapper.Object
        );
    }

    [Fact]
    public async Task GetRecentlyVisitedHotelsAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var hotels = Enumerable.Range(0, 3)
            .Select(_ => _fixture.Build<Hotel>()
                .Without(h => h.City)
                .Without(h => h.Owner)
                .Without(h => h.RoomCategories)
                .Without(h => h.Reviews)
                .Without(h => h.Gallery)
                .Without(h => h.Bookings)
                .Create())
            .ToList();

        _repoMock.Setup(r => r.GetRecentlyVisitedHotelsAsync(userId, 3))
                 .ReturnsAsync(hotels);

        _mapperMock.Setup(m => m.ToRecentlyVisitedHotelDto(It.IsAny<Hotel>()))
                   .Returns(() => _fixture.Create<RecentlyVisitedHotelDto>());

        // Act
        var result = await _sut.GetRecentlyVisitedHotelsAsync(userId, 3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        _repoMock.Verify(r => r.GetRecentlyVisitedHotelsAsync(userId, 3), Times.Once);
        _mapperMock.Verify(m => m.ToRecentlyVisitedHotelDto(It.IsAny<Hotel>()), Times.Exactly(3));
    }

    [Fact]
    public async Task GetRecentlyVisitedHotelsAsync_ShouldReturnEmpty_WhenNoHotelsFound()
    {
        _repoMock.Setup(r => r.GetRecentlyVisitedHotelsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                 .ReturnsAsync(new List<Hotel>());

        var result = await _sut.GetRecentlyVisitedHotelsAsync(Guid.NewGuid(), 5);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task GetRecentlyVisitedHotelsAsync_ShouldPropagateException()
    {
        _repoMock.Setup(r => r.GetRecentlyVisitedHotelsAsync(It.IsAny<Guid>(), It.IsAny<int>()))
                 .ThrowsAsync(new Exception("DB failed"));

        Func<Task> act = async () => await _sut.GetRecentlyVisitedHotelsAsync(Guid.NewGuid(), 4);

        await act.Should().ThrowAsync<Exception>().WithMessage("DB failed");
    }
}