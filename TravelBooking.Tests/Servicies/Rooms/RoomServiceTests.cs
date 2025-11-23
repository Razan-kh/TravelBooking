using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Application.AddingToCart.Services.Implementations;
using TravelBooking.Application.AddingToCart.Services.Implementations;
using TravelBooking.Application.AddingToCart.Handlers;
using TravelBooking.Application.AddingToCart.Services.Implementations;
using TravelBooking.Application.AddingToCart.Services.Interfaces;
using TravelBooking.Application.AddingToCart.Mappers;
using TravelBooking.Application.AddingToCart.Mappers;

using TravelBooking.Application.AddingToCart.Mappers;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.ViewingHotels.Services.Implementations;
using TravelBooking.Application.ViewingHotels.Mappers;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Rooms.Interfaces;

namespace TravelBooking.Tests.Services;

public class RoomServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomRepository> _roomRepoMock;
    private readonly Mock<IRoomCategoryMapper> _mapperMock;
    private readonly RoomService _sut;

    public RoomServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _roomRepoMock = _fixture.Freeze<Mock<IRoomRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IRoomCategoryMapper>>();

        _sut = new RoomService(_roomRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetRoomCategoriesWithAvailabilityAsync_WhenDatesNull_UsesCountTotalRooms()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var rc1 = new RoomCategory { Id = Guid.NewGuid(), Name = "R1" };
        var rc2 = new RoomCategory { Id = Guid.NewGuid(), Name = "R2" };
        var rcList = new List<RoomCategory> { rc1, rc2 };

        _roomRepoMock.Setup(r => r.GetRoomCategoriesByHotelIdAsync(hotelId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(rcList);

        // mapper returns basic DTOs
        _mapperMock.Setup(m => m.Map(It.IsAny<RoomCategory>()))
                   .Returns<RoomCategory>(rc => new RoomCategoryDto { Id = rc.Id, Name = rc.Name });

        // CountTotalRooms called for each category
        _roomRepoMock.Setup(r => r.CountTotalRoomsAsync(rc1.Id, It.IsAny<CancellationToken>())).ReturnsAsync(5);
        _roomRepoMock.Setup(r => r.CountTotalRoomsAsync(rc2.Id, It.IsAny<CancellationToken>())).ReturnsAsync(2);

        // Act
        var result = await _sut.GetRoomCategoriesWithAvailabilityAsync(hotelId, null, null, CancellationToken.None);

        // Assert
        result.Should().HaveCount(2);
        result.Single(r => r.Id == rc1.Id).AvailableRooms.Should().Be(5);
        result.Single(r => r.Id == rc2.Id).AvailableRooms.Should().Be(2);

        _roomRepoMock.Verify(r => r.CountTotalRoomsAsync(rc1.Id, It.IsAny<CancellationToken>()), Times.Once);
        _roomRepoMock.Verify(r => r.CountTotalRoomsAsync(rc2.Id, It.IsAny<CancellationToken>()), Times.Once);
        _roomRepoMock.Verify(r => r.CountAvailableRoomsAsync(It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetRoomCategoriesWithAvailabilityAsync_WhenDatesProvided_UsesCountAvailableRooms()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        var rc = new RoomCategory { Id = Guid.NewGuid(), Name = "R1" };
        var rcList = new List<RoomCategory> { rc };

        var checkIn = new DateOnly(2025, 11, 21);
        var checkOut = new DateOnly(2025, 11, 22);

        _roomRepoMock.Setup(r => r.GetRoomCategoriesByHotelIdAsync(hotelId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(rcList);

        _mapperMock.Setup(m => m.Map(It.IsAny<RoomCategory>()))
                   .Returns<RoomCategory>(r => new RoomCategoryDto { Id = r.Id, Name = r.Name });

        _roomRepoMock.Setup(r => r.CountAvailableRoomsAsync(rc.Id, checkIn, checkOut, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(3);

        // Act
        var result = await _sut.GetRoomCategoriesWithAvailabilityAsync(hotelId, checkIn, checkOut, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].AvailableRooms.Should().Be(3);

        _roomRepoMock.Verify(r => r.CountAvailableRoomsAsync(rc.Id, checkIn, checkOut, It.IsAny<CancellationToken>()), Times.Once);
        _roomRepoMock.Verify(r => r.CountTotalRoomsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetRoomCategoriesWithAvailabilityAsync_WhenRepoReturnsEmpty_ReturnsEmptyList()
    {
        // Arrange
        var hotelId = Guid.NewGuid();
        _roomRepoMock.Setup(r => r.GetRoomCategoriesByHotelIdAsync(hotelId, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<RoomCategory>());

        // Act
        var result = await _sut.GetRoomCategoriesWithAvailabilityAsync(hotelId, null, null, CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}