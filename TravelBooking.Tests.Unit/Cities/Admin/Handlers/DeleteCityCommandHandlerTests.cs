using Moq;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using FluentAssertions;
using TravelBooking.Application.Cities.Handlers;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.Cities.Admin.Handlers;

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
    public async Task Handle_ShouldReturnNoContent_WhenServiceDeletes()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Mock city object returned from repo
        var city = new City { Id = id, Name = "Test City", PostalCode = "p400", Country = "country" };

        _serviceMock
            .Setup(s => s.DeleteCityAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success()); // Mock service to return success

        // Act
        var result = await _handler.Handle(new DeleteCityCommand(id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HttpStatusCode.Should().Be(204); // assuming Result.Success() sets 204 internally

        _serviceMock.Verify(s => s.DeleteCityAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}