using Moq;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Interfaces.Servicies;
using FluentAssertions;
using TravelBooking.Application.Cities.Handlers;

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
    public async Task Handle_ShouldReturnSuccess_WhenServiceDeletes()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = await _handler.Handle(new DeleteCityCommand(id), CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteCityAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}