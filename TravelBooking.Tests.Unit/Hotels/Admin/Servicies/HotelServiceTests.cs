using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Admin.Servicies.Implementations;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Mappers.Interfaces;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Tests.Shared;

namespace TravelBooking.Tests.Hotels.Servicies;

public class HotelServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IHotelRepository> _repoMock;
    private readonly Mock<IHotelMapper> _mapperMock;
    private readonly HotelService _service;

    public HotelServiceTests()
    {
        _fixture = new Fixture().Customize(new EntityCustomization());
        _repoMock = new Mock<IHotelRepository>();
        _mapperMock = new Mock<IHotelMapper>();
        _service = new HotelService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateHotelAsync_ShouldReturnMappedDto_WhenHotelCreated()
    {
        // Arrange
        var dto = _fixture.Create<CreateHotelDto>();
        var hotelEntity = _fixture.Create<Hotel>();
        var hotelDto = _fixture.Create<HotelDto>();

        _mapperMock.Setup(m => m.Map(dto)).Returns(hotelEntity);
        _mapperMock.Setup(m => m.Map(hotelEntity)).Returns(hotelDto);
        _repoMock.Setup(r => r.AddAsync(hotelEntity, It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        // Act
        var result = await _service.CreateHotelAsync(dto, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(hotelDto);
        _repoMock.Verify(r => r.AddAsync(hotelEntity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldCallDelete_WhenHotelExists()
    {
        // Arrange
        var hotel = _fixture.Create<Hotel>();
        _repoMock.Setup(r => r.GetByIdAsync(hotel.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(hotel);
        // Act
        await _service.DeleteHotelAsync(hotel.Id, CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(hotel, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteHotelAsync_ShouldReturnFailure_WhenHotelDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();

        _repoMock
            .Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Hotel?)null);

        // Act
        var result = await _service.DeleteHotelAsync(id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
        result.ErrorCode.Should().Be("NOT_FOUND");
        result.HttpStatusCode.Should().Be(404);

        _repoMock.Verify(
            r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()),
            Times.Once
        );

        _repoMock.Verify(
            r => r.DeleteAsync(It.IsAny<Hotel>(), It.IsAny<CancellationToken>()),
            Times.Never
        );
    }
}