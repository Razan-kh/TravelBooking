using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using TravelBooking.Application.Cities.Handlers;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using TravelBooking.Application.Cities.Servicies.Implementations;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Cities.Entities;
using Xunit;

public class CityServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICityRepository> _repoMock;
    private readonly Mock<ICityMapper> _mapperMock;
    private readonly CityService _service;

    public CityServiceTests()
    {
        _fixture = new Fixture();
        _fixture.Customize(new EntityCustomization()); 
        _repoMock = _fixture.Freeze<Mock<ICityRepository>>();
        _mapperMock = _fixture.Freeze<Mock<ICityMapper>>();
        _service = new CityService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateCityAsync_ShouldReturnMappedDto()
    {
        var dto = _fixture.Create<CreateCityDto>();
        var entity = _fixture.Create<City>();
        var mappedDto = _fixture.Create<CityDto>();

        _mapperMock.Setup(m => m.Map(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map(entity)).Returns(mappedDto);

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