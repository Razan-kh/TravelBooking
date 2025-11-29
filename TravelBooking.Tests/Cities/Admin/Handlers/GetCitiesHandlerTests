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
using TravelBooking.Application.Cities.Servicies;
using TravelBooking.Application.Mappers;
using Xunit;

namespace TravelBooking.Tests.Cities.Admin.Handlers;

public class GetCitiesHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICityService> _serviceMock;
    private readonly GetCitiesHandler _handler;

    public GetCitiesHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _serviceMock = _fixture.Freeze<Mock<ICityService>>();
        _handler = new GetCitiesHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedCities_WhenCitiesExist()
    {
        var cities = _fixture.CreateMany<CityDto>(5).ToList();
        _serviceMock.Setup(s => s.GetCitiesAsync(null, 1, 20, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(cities);

        var result = await _handler.Handle(new GetCitiesQuery(null, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().HaveCount(5);
        result.Value.Data.Should().HaveCount(5);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResult_WhenNoCitiesExist()
    {
        var emptyList = new List<CityDto>();
        _serviceMock.Setup(s => s.GetCitiesAsync(null, 1, 20, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(emptyList);

        var result = await _handler.Handle(new GetCitiesQuery (null, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().BeEmpty();
        result.Value.Data.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_ShouldApplyFilter_WhenFilterIsProvided()
    {
        var filter = "Paris";
        var cities = _fixture.CreateMany<CityDto>(2).ToList();

        _serviceMock.Setup(s => s.GetCitiesAsync(filter, 1, 20, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(cities);

        var result = await _handler.Handle(new GetCitiesQuery(filter, 1, 20), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().HaveCount(2);
        _serviceMock.Verify(s => s.GetCitiesAsync(filter, 1, 20, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(0, 0, 1, 20)]  // page <= 0 and pageSize <= 0 => fallback
    [InlineData(-5, -10, 1, 20)]
    [InlineData(1, 0, 1, 20)]
    [InlineData(0, 5, 1, 5)]
    public async Task Handle_ShouldCorrectInvalidPageAndPageSize(int page, int pageSize, int expectedPage, int expectedPageSize)
    {
        var cities = _fixture.CreateMany<CityDto>(3).ToList();
        _serviceMock.Setup(s => s.GetCitiesAsync(null, expectedPage, expectedPageSize, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(cities);

        var result = await _handler.Handle(new GetCitiesQuery(null, page, pageSize), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().HaveCount(3);
        _serviceMock.Verify(s => s.GetCitiesAsync(null, expectedPage, expectedPageSize, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldRespectCancellationToken()
    {
        var cts = new CancellationTokenSource();
        var cities = _fixture.CreateMany<CityDto>(2).ToList();
        _serviceMock.Setup(s => s.GetCitiesAsync(null, 1, 20, cts.Token))
                    .ReturnsAsync(cities);

        await _handler.Handle(new GetCitiesQuery(null, 1, 20), cts.Token);

        _serviceMock.Verify(s => s.GetCitiesAsync(null, 1, 20, cts.Token), Times.Once);
    }
}