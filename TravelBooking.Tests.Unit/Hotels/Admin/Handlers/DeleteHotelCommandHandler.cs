using FluentAssertions;
using Moq;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Handlers;
using TravelBooking.Application.Hotels.Servicies;

namespace TravelBooking.Tests.Hotels.Handlers;

public class DeleteHotelCommandHandlerTests
{
    private readonly Mock<IHotelService> _serviceMock;
    private readonly DeleteHotelCommandHandler _handler;

    public DeleteHotelCommandHandlerTests()
    {
        _serviceMock = new Mock<IHotelService>();
        _handler = new DeleteHotelCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCallServiceDelete_WhenHotelExists()
    {
        var id = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteHotelAsync(id, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

        var result = await _handler.Handle(new DeleteHotelCommand(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.DeleteHotelAsync(id, It.IsAny<CancellationToken>()), Times.Once);
    }
}