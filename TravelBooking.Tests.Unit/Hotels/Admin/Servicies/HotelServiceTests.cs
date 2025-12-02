using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Mappers.Interfaces;
using TravelBooking.Application.Hotels.Servicies;
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
    public async Task UpdateHotelAsync_ShouldThrowKeyNotFound_WhenHotelDoesNotExist()
    {
        var dto = _fixture.Create<UpdateHotelDto>();
        _repoMock.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((Hotel?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.UpdateHotelAsync(dto, CancellationToken.None));
    }
}