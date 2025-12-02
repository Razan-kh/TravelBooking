
using TravelBooking.Tests.Carts.TestHelpers;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;

namespace TravelBooking.Tests.Unit.Cheackout.Servicies;

public class BookingServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly Mock<IDiscountService> _discountService;
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _bookingRepoMock = _fixture.Freeze<Mock<IBookingRepository>>();
        _discountService = _fixture.Freeze<Mock<IDiscountService>>();

        _sut = new BookingService(_bookingRepoMock.Object, _discountService.Object);
    }

    [Fact]
    public async Task CreateBookingsAsync_MultipleItems_GroupedByHotel_CreatesBookingForEachHotel()
    {
        // Arrange
        var command = _fixture.Create<CheckoutCommand>();
        var x = _fixture.CreateValidCart(2);

        // Act
        var result = await _sut.CreateBookingsAsync(x, command, CancellationToken.None);

        // Assert
        result.Should().HaveCount(x.Items.Select(i => i.RoomCategory.HotelId).Distinct().Count());
        _bookingRepoMock.Verify(r => r.AddAsync(It.IsAny<Booking>(), It.IsAny<CancellationToken>()), Times.Exactly(result.Count));
    }
}
