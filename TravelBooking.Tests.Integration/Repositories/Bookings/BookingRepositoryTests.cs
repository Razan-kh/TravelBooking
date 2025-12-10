using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using Xunit;
using AutoFixture;
using AutoFixture.Xunit2;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Tests.Integration.Seeders;
using TravelBooking.Tests.Integration.Builders;
using TravelBooking.Tests.Integration.Models;

namespace TravelBooking.Tests.Integration.Repositories.Bookings;

public class BookingRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly BookingRepository _repository;
    private readonly BookingTestDataSeeder _seeder;
    private readonly TestDataBuilder _dataBuilder;
    private BookingTestData _testData;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BookingTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new BookingRepository(_dbContext);
        _dataBuilder = new TestDataBuilder();
        _seeder = new BookingTestDataSeeder(_dbContext);
    }

    public async Task InitializeAsync()
    {
        _testData = await _seeder.SeedBasicTestDataAsync();
    }

    public Task DisposeAsync()
    {
        _dbContext?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddAsync_ValidBooking_AddsToDatabase()
    {
        // Arrange
        var newBooking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = _testData.UserId,
            HotelId = _testData.HotelId,
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(12)),
            BookingDate = DateTime.UtcNow,
            GuestRemarks = "Test booking remarks",
            PaymentDetails = _dataBuilder.CreatePaymentDetails(300.00m, PaymentMethod.Card)
        };

        var initialCount = await _dbContext.Bookings.CountAsync();
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.AddAsync(newBooking, cancellationToken);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCount = await _dbContext.Bookings.CountAsync();
        finalCount.Should().Be(initialCount + 1);

        var savedBooking = await _dbContext.Bookings.FindAsync(newBooking.Id);
        savedBooking.Should().NotBeNull();
        savedBooking!.UserId.Should().Be(_testData.UserId);
        savedBooking.HotelId.Should().Be(_testData.HotelId);
    }

    [Theory, AutoData]
    public async Task AddAsync_ValidBookingWithAutoData_AddsToDatabase(
        string guestRemarks,
        decimal paymentAmount,
        PaymentMethod paymentMethod)
    {
        // Arrange
        var newBooking = _dataBuilder.Fixture.Build<Booking>()
            .With(b => b.UserId, _testData.UserId)
            .With(b => b.HotelId, _testData.HotelId)
            .With(b => b.GuestRemarks, guestRemarks)
            .With(b => b.PaymentDetails, _dataBuilder.CreatePaymentDetails(paymentAmount, paymentMethod))
            .Create();

        var initialCount = await _dbContext.Bookings.CountAsync();
        var cancellationToken = CancellationToken.None;

        // Act
        await _repository.AddAsync(newBooking, cancellationToken);
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCount = await _dbContext.Bookings.CountAsync();
        finalCount.Should().Be(initialCount + 1);

        var savedBooking = await _dbContext.Bookings.FindAsync(newBooking.Id);
        savedBooking.Should().NotBeNull();
        savedBooking!.GuestRemarks.Should().Be(guestRemarks);
        savedBooking.PaymentDetails.Amount.Should().Be(paymentAmount);
        savedBooking.PaymentDetails.PaymentMethod.Should().Be(paymentMethod);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingBooking_ReturnsBooking()
    {
        // Arrange
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByIdAsync(_testData.Booking.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(_testData.Booking.Id);
        result.UserId.Should().Be(_testData.Booking.UserId);
        result.HotelId.Should().Be(_testData.Booking.HotelId);
        result.GuestRemarks.Should().Be(_testData.Booking.GuestRemarks);
        result.PaymentDetails.Should().NotBeNull();
        result.PaymentDetails.Amount.Should().Be(_testData.Booking.PaymentDetails.Amount);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingBooking_ReturnsNull()
    {
        // Arrange
        var nonExistingBookingId = Guid.NewGuid();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByIdAsync(nonExistingBookingId, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WithCancelledToken_ThrowsOperationCanceledException()
    {
        // Arrange
        var cancelledToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.GetByIdAsync(_testData.Booking.Id, cancelledToken));
    }

    [Fact]
    public async Task AddAsync_NullBooking_ThrowsArgumentNullException()
    {
        // Arrange
        Booking nullBooking = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.AddAsync(nullBooking, cancellationToken));
    }

    [Theory]
    [InlineData(PaymentMethod.Card)]
    [InlineData(PaymentMethod.Cash)]
    public async Task GetByIdAsync_BookingWithDifferentPaymentMethods_ReturnsCorrectPaymentDetails(
        PaymentMethod paymentMethod)
    {
        // Arrange
        var booking = await _seeder.AddAdditionalBookingAsync(_testData.UserId, _testData.HotelId, b =>
        {
            b.PaymentDetails = _dataBuilder.CreatePaymentDetails(300.00m, paymentMethod);
        });

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByIdAsync(booking.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.PaymentDetails.PaymentMethod.Should().Be(paymentMethod);
    }

    [Fact]
    public async Task AddAsync_MultipleBookings_AllAddedSuccessfully()
    {
        // Arrange
        var bookings = _dataBuilder.Fixture.Build<Booking>()
            .With(b => b.UserId, _testData.UserId)
            .With(b => b.HotelId, _testData.HotelId)
            .CreateMany(5)
            .ToList();

        var initialCount = await _dbContext.Bookings.CountAsync();
        var cancellationToken = CancellationToken.None;

        // Act
        foreach (var booking in bookings)
        {
            await _repository.AddAsync(booking, cancellationToken);
        }
        await _dbContext.SaveChangesAsync();

        // Assert
        var finalCount = await _dbContext.Bookings.CountAsync();
        finalCount.Should().Be(initialCount + 5);

        foreach (var booking in bookings)
        {
            var savedBooking = await _dbContext.Bookings.FindAsync(booking.Id);
            savedBooking.Should().NotBeNull();
        }
    }

    [Fact]
    public async Task GetByIdAsync_BookingWithFutureDates_ReturnsCorrectDates()
    {
        // Arrange
        var futureCheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        var futureCheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(35));

        var booking = await _seeder.AddAdditionalBookingAsync(_testData.UserId, _testData.HotelId, b =>
        {
            b.CheckInDate = futureCheckIn;
            b.CheckOutDate = futureCheckOut;
        });

        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByIdAsync(booking.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.CheckInDate.Should().Be(futureCheckIn);
        result.CheckOutDate.Should().Be(futureCheckOut);
        (result.CheckOutDate.DayNumber - result.CheckInDate.DayNumber).Should().Be(5);
    }
}