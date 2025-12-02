using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Application.DTOs;
using TravelBooking.Tests.Carts.TestHelpers;
using TravelBooking.Application.Reviews.Services.Implementations;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;

namespace TravelBooking.Tests.Services;

public class ReviewServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IReviewRepository> _repoMock;
    private readonly Mock<IHotelMapper> _mapperMock;
    private readonly ReviewService _sut;

    public ReviewServiceTests()
    {
        _fixture = FixtureFactory.Create();

        _repoMock = _fixture.Freeze<Mock<IReviewRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IHotelMapper>>();

        _sut = new ReviewService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetHotelReviewsAsync_ReturnsMappedDtos()
    {
        // Arrange
        var hotelId = Guid.NewGuid();

        var domainReviews = _fixture.CreateMany<Review>(2).ToList();
        var dtoReviews = _fixture.CreateMany<ReviewDto>(2).ToList();

        _repoMock
            .Setup(r => r.GetByHotelIdAsync(hotelId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainReviews);

        _mapperMock
            .Setup(m => m.MapReviews(domainReviews))
            .Returns(dtoReviews);

        // Act
        var result = await _sut.GetHotelReviewsAsync(hotelId, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(dtoReviews);

        _repoMock.Verify(r => r.GetByHotelIdAsync(hotelId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.MapReviews(domainReviews), Times.Once);
    }
}
