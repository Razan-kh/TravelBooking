using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;
using TravelBooking.Application.Hotels.User.Services.Implementations;

namespace TravelBooking.Tests.Application.RecentlyVisited;

public class HotelService_RecentlyVisited_Tests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly Mock<IRecentlyVisitedHotelMapper> _recentlyMapper;
    private readonly HotelService _sut;

    public HotelService_RecentlyVisited_Tests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _repoMock = _fixture.Freeze<Mock<IHotelRepository>>();
        _recentlyMapper = _fixture.Freeze<Mock<IRecentlyVisitedHotelMapper>>();
        var featuredHotelsMapper = _fixture.Freeze<Mock<IFeaturedHotelMapper>>();
        var HotelMapper = _fixture.Freeze<Mock<IHotelMapper>>();

        _sut = new HotelService(
            _repoMock.Object,
            HotelMapper.Object,
            featuredHotelsMapper.Object,
            _recentlyMapper.Object
        );
    }

    [Fact]
    public async Task GetRecentlyVisitedHotelsAsync_ShouldReturnMappedResults()
    {
        // Arrange
        var userId = Guid.NewGuid();

        var hotels = _fixture.Build<Hotel>()
                     .With(h => h.City, _fixture.Build<City>()
                                                  .With(c => c.Name, "Test City")
                                                  .Without(c => c.Hotels)
                                                  .Create())
                     .Without(h => h.Owner)
                     .Without(h => h.RoomCategories)
                     .Without(h => h.Reviews)
                     .Without(h => h.Gallery)
                     .Without(h => h.Bookings)
                     .CreateMany(3)
                     .ToList();

        _repoMock.Setup(r => r.GetRecentlyVisitedHotelsAsync(userId, 3))
                 .ReturnsAsync(hotels);

        _recentlyMapper.Setup(m => m.ToRecentlyVisitedHotelDto(It.IsAny<Hotel>()))
                   .Returns(() => _fixture.Create<RecentlyVisitedHotelDto>());

        // Act
        var result = await _sut.GetRecentlyVisitedHotelsAsync(userId, 3);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(3);

        _repoMock.Verify(r => r.GetRecentlyVisitedHotelsAsync(userId, 3), Times.Once);
        _recentlyMapper.Verify(m => m.ToRecentlyVisitedHotelDto(It.IsAny<Hotel>()), Times.Exactly(3));
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