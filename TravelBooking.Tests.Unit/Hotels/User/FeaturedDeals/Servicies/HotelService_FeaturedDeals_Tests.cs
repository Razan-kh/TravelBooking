using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Domain.Hotels;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;
using TravelBooking.Application.Hotels.User.Services.Implementations;

namespace TravelBooking.Tests.FeaturedDeals;

public class HotelService_FeaturedDeals_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly Mock<IFeaturedHotelMapper> _featuredHotelsMapper;
    private readonly Mock<IRecentlyVisitedHotelMapper> _recentlyMapper;
    private readonly HotelService _sut;

    public HotelService_FeaturedDeals_Tests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _repoMock = _fixture.Freeze<Mock<IHotelRepository>>();
        _recentlyMapper = _fixture.Freeze<Mock<IRecentlyVisitedHotelMapper>>();
        _featuredHotelsMapper = _fixture.Freeze<Mock<IFeaturedHotelMapper>>();
        var _hotelMapper = _fixture.Freeze<Mock<IHotelMapper>>();


        _sut = new HotelService(
            _repoMock.Object,
            _hotelMapper.Object,
            _featuredHotelsMapper.Object,
            _recentlyMapper.Object
        );
    }

    [Fact]
    public async Task GetFeaturedDealsAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var city =
        _fixture.Build<City>()
        .Without(h => h.Hotels)
        .Create();

        var hotel =
        _fixture.Build<Hotel>()
        .With(h => h.City, city)
        .Without(h => h.Owner)
        .Without(h => h.RoomCategories)
        .Without(h => h.Reviews)
        .Without(h => h.Gallery)
        .Without(h => h.Bookings)
        .Create();

        var entities = Enumerable.Range(0, 3)
        .Select(_ => _fixture.Build<HotelWithMinPrice>()
            .With(h => h.Hotel, hotel)
            .With(h => h.DiscountedPrice, 10)
            .With(h => h.MinPrice, 10)
            .Create())
        .ToList();
        
        _repoMock.Setup(r => r.GetFeaturedHotelsAsync(3))
                 .ReturnsAsync(entities);

        // Act
        var result = await _sut.GetFeaturedDealsAsync(3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        _repoMock.Verify(r => r.GetFeaturedHotelsAsync(3), Times.Once);
        _featuredHotelsMapper.Verify(m => m.ToFeaturedHotelDto(It.IsAny<HotelWithMinPrice>()), Times.Exactly(3));
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