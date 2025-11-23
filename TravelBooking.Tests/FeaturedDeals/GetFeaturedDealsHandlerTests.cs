using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.FeaturedDeals.Handlers;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.Services.Interfaces;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.FeaturedDeals;

public class GetFeaturedDealsHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHomeService> _homeServiceMock;
    private readonly GetFeaturedDealsHandler _sut;

    public GetFeaturedDealsHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _homeServiceMock = _fixture.Freeze<Mock<IHomeService>>();
        _sut = new GetFeaturedDealsHandler(_homeServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccessResult_WhenServiceReturnsData()
    {
        // Arrange
        var count = 5;
        var expected = _fixture.CreateMany<FeaturedHotelDto>(count).ToList();

        _homeServiceMock
            .Setup(s => s.GetFeaturedDealsAsync(count))
            .ReturnsAsync(Result.Success(expected));

        var query = new GetFeaturedDealsQuery(count);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(count);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenServiceReturnsFailure()
    {
        // Arrange
        var count = 3;

        _homeServiceMock
            .Setup(s => s.GetFeaturedDealsAsync(count))
            .ReturnsAsync(Result.Failure<List<FeaturedHotelDto>>("ERR"));

        var query = new GetFeaturedDealsQuery(count);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("ERR");
    }
}