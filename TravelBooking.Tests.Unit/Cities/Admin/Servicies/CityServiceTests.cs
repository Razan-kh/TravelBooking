using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Cities.Mappers.Interfaces;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Tests.Shared;
using TravelBooking.Domain.Cities.Interfaces;

namespace TravelBooking.Tests.Services;

public class CityServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICityRepository> _repoMock;
    private readonly Mock<ICityMapper> _cityMapperMock;
    private readonly Mock<ITrendingCityMapper> _trendingCityMapperMock;
    private readonly CityService _service;

    public CityServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new EntityCustomization());

        _repoMock = _fixture.Freeze<Mock<ICityRepository>>();
        _cityMapperMock = _fixture.Freeze<Mock<ICityMapper>>();
        var trendingCityMapperMock = new Mock<ITrendingCityMapper>();

        trendingCityMapperMock
            .Setup(m => m.ToTrendingCityDto(It.IsAny<(City city, int visitCount)>()))
            .Returns(((City city, int visitCount) c) => new TrendingCityDto
            {
                Id = c.city.Id,
                Name = c.city.Name,
                VisitCount = c.visitCount
            });

        _trendingCityMapperMock = trendingCityMapperMock;
        _service = new CityService(_repoMock.Object,
        _cityMapperMock.Object, _trendingCityMapperMock.Object);
    }

    [Fact]
    public async Task CreateCityAsync_ShouldReturnMappedDto()
    {
        var dto = _fixture.Create<CreateCityDto>();
        var entity = _fixture.Create<City>();
        var mappedDto = _fixture.Create<CityDto>();

        _cityMapperMock.Setup(m => m.Map(dto)).Returns(entity);
        _cityMapperMock.Setup(m => m.Map(entity)).Returns(mappedDto);

        var result = await _service.CreateCityAsync(dto, CancellationToken.None);

        result.Should().BeEquivalentTo(mappedDto);
        _repoMock.Verify(r => r.AddAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCityAsync_ShouldThrow_WhenCityNotFound()
    {
        var dto = _fixture.Create<UpdateCityDto>();
        _repoMock.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync((City?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateCityAsync(dto, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteCityAsync_ShouldCallDelete_WhenCityExists()
    {
        var city = _fixture.Create<City>();
        _repoMock.Setup(r => r.GetByIdAsync(city.Id, It.IsAny<CancellationToken>()))
                 .ReturnsAsync(city);

        await _service.DeleteCityAsync(city.Id, CancellationToken.None);

        _repoMock.Verify(r => r.DeleteAsync(city, It.IsAny<CancellationToken>()), Times.Once);
    }
}