using Moq;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Interfaces.Servicies;
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

public class DeleteCityCommandHandlerTests
{
    private readonly Mock<ICityService> _serviceMock;
    private readonly DeleteCityCommandHandler _handler;

    public DeleteCityCommandHandlerTests()
    {
        _serviceMock = new Mock<ICityService>();
        _handler = new DeleteCityCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSuccess_WhenServiceDeletes()
    {
        var id = Guid.NewGuid();
        var result = await _handler.Handle(new DeleteCityCommand(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteCityAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}