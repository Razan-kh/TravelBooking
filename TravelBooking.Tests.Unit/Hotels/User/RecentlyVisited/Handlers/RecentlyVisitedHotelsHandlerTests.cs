using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.User.Services.Interfaces;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.RecentlyVisited.Handlers;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.RecentlyVisited;

public class RecentlyVisitedHotelsHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelService> _HotelServiceMock;
    private readonly RecentlyVisitedHotelsHandler _sut;

    public RecentlyVisitedHotelsHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _HotelServiceMock = _fixture.Freeze<Mock<IHotelService>>();
        _sut = new RecentlyVisitedHotelsHandler(_HotelServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenServiceReturnsData()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var count = 3;
        var dtoList = _fixture.CreateMany<RecentlyVisitedHotelDto>(count).ToList();

        _HotelServiceMock
            .Setup(s => s.GetRecentlyVisitedHotelsAsync(userId, count))
            .ReturnsAsync(Result.Success(dtoList));

        var query = new GetRecentlyVisitedHotelsQuery(userId, count);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(dtoList);
    }

    [Fact]
    public async Task Handle_ShouldReturnFailure_WhenServiceFails()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _HotelServiceMock
            .Setup(s => s.GetRecentlyVisitedHotelsAsync(userId, 5))
            .ReturnsAsync(Result.Failure<List<RecentlyVisitedHotelDto>>("Failed"));

        var query = new GetRecentlyVisitedHotelsQuery(userId, 5);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Failed");
    }
}