using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.ViewingHotels.Queries;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Application.ViewingHotels.Handlers;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.Rooms.User.Servicies.Interfaces;
using TravelBooking.Application.Reviews.Services.Interfaces;

namespace TravelBooking.Tests.Handlers;

public class GetHotelDetailsHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelService> _hotelServiceMock;
    private readonly Mock<IRoomService> _roomServiceMock;
    private readonly Mock<IReviewService> _reviewServiceMock;
    private readonly GetHotelDetailsHandler _sut;

    public GetHotelDetailsHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        _hotelServiceMock = _fixture.Freeze<Mock<IHotelService>>();
        _roomServiceMock = _fixture.Freeze<Mock<IRoomService>>();
        _reviewServiceMock = _fixture.Freeze<Mock<IReviewService>>();

        _sut = new GetHotelDetailsHandler(
            _hotelServiceMock.Object,
            _roomServiceMock.Object,
            _reviewServiceMock.Object);
    }

    [Fact]
    public async Task Handle_HotelDoesNotExist_ReturnsFailureResult()
    {
        // Arrange
        var query = new GetHotelDetailsQuery(
            HotelId: Guid.NewGuid(),
            CheckIn: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOut: DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        );
        _hotelServiceMock.Setup(s => s.GetHotelDetailsAsync(query.HotelId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync((HotelDetailsDto?)null);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Hotel not found");
        result.HttpStatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Handle_HotelExists_ReturnsHotelDetailsWithReviewsAndRooms()
    {
        // Arrange
        var query = new GetHotelDetailsQuery(
            HotelId: Guid.NewGuid(),
            CheckIn: DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            CheckOut: DateOnly.FromDateTime(DateTime.Today.AddDays(3))
        );
        var hotelDto = _fixture.Create<HotelDetailsDto>();
        var reviews = _fixture.CreateMany<ReviewDto>(3).ToList();
        var rooms = _fixture.CreateMany<RoomCategoryDto>(2).ToList();

        _hotelServiceMock.Setup(s => s.GetHotelDetailsAsync(query.HotelId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(hotelDto);
        _reviewServiceMock.Setup(s => s.GetHotelReviewsAsync(query.HotelId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(reviews);
        _roomServiceMock.Setup(s => s.GetRoomCategoriesWithAvailabilityAsync(
                                    query.HotelId,
                                    query.CheckIn,
                                    query.CheckOut,
                                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(rooms);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(hotelDto, options => options.Excluding(h => h.Reviews).Excluding(h => h.RoomCategories));
        result.Value.Reviews.Should().BeEquivalentTo(reviews);
        result.Value.RoomCategories.Should().BeEquivalentTo(rooms);

        _hotelServiceMock.Verify(s => s.GetHotelDetailsAsync(query.HotelId, It.IsAny<CancellationToken>()), Times.Once);
        _reviewServiceMock.Verify(s => s.GetHotelReviewsAsync(query.HotelId, It.IsAny<CancellationToken>()), Times.Once);
        _roomServiceMock.Verify(s => s.GetRoomCategoriesWithAvailabilityAsync(
                                    query.HotelId, query.CheckIn, query.CheckOut, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_HotelExists_WithNullCheckInCheckOut_ReturnsRoomsWithoutAvailabilityCheck()
    {
        // Arrange
        var query = new GetHotelDetailsQuery(Guid.NewGuid(), null, null);
        var hotelDto = _fixture.Create<HotelDetailsDto>();
        var reviews = _fixture.CreateMany<ReviewDto>(2).ToList();
        var rooms = _fixture.CreateMany<RoomCategoryDto>(2).ToList();

        _hotelServiceMock.Setup(s => s.GetHotelDetailsAsync(query.HotelId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(hotelDto);
        _reviewServiceMock.Setup(s => s.GetHotelReviewsAsync(query.HotelId, It.IsAny<CancellationToken>()))
                            .ReturnsAsync(reviews);
        _roomServiceMock.Setup(s => s.GetRoomCategoriesWithAvailabilityAsync(
                                    query.HotelId,
                                    null,
                                    null,
                                    It.IsAny<CancellationToken>()))
                        .ReturnsAsync(rooms);

        // Act
        var result = await _sut.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.RoomCategories.Should().BeEquivalentTo(rooms);

        _roomServiceMock.Verify(s => s.GetRoomCategoriesWithAvailabilityAsync(
                                    query.HotelId, null, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}