using TravelBooking.Application.Carts.Handlers;
using TravelBooking.Application.Carts.Commands;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Tests.AddingToCart.TestHelpers;
using TravelBooking.Application.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Domain.Bookings.Entities;
using Xunit;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Bookings.Repositories;

public class BookingServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IBookingRepository> _bookingRepoMock;
    private readonly BookingService _sut;

    public BookingServiceTests()
    {
        //  _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        // Add this to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _bookingRepoMock = _fixture.Freeze<Mock<IBookingRepository>>();
        _sut = new BookingService(_bookingRepoMock.Object);
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
