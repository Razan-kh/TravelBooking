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

namespace TravelBooking.Tests.Integration.Repositories;

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
/*
public class BookingRepositoryTests : IAsyncLifetime
{
    private readonly AppDbContext _dbContext;
    private readonly BookingRepository _repository;
    private readonly IFixture _fixture;
    private readonly Guid _testUserId;
    private readonly Guid _testHotelId;
    private readonly Guid _testRoomCategoryId;
    private readonly Guid _testRoomId;

    public BookingRepositoryTests()
    {
        _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
.ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        // Customize fixture to avoid circular references and set up realistic data
        _fixture.Customize<Booking>(composer => composer
            .Without(b => b.User)
            .Without(b => b.Hotel)
            .Without(b => b.Rooms));

        _fixture.Customize<PaymentDetails>(composer => composer);

        _fixture.Customize<Room>(composer => composer
            .Without(r => r.Bookings)
            .Without(r => r.RoomCategory)
            .Without(r => r.Gallery));

        _fixture.Customize<RoomCategory>(composer => composer
            .Without(rc => rc.Rooms)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts));

        _fixture.Customize<Hotel>(composer => composer
            .Without(h => h.RoomCategories)
            .Without(h => h.Reviews)
            .Without(h => h.Gallery)
            .Without(h => h.Bookings)
            .Without(h => h.City)
            .Without(h => h.Owner));

        _fixture.Customize<User>(composer => composer
            .Without(u => u.Bookings));

        // Configure DateTime properties
        _fixture.Customize<DateTime>(composer => composer
            .FromFactory(() => DateTime.UtcNow));

        _fixture.Customize<DateOnly>(composer => composer
            .FromFactory(() => DateOnly.FromDateTime(DateTime.UtcNow)));

        _fixture.Customize<DateOnly>(composer => composer
            .FromFactory<DateTime>(dt => DateOnly.FromDateTime(dt)));

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"BookingTestDb_{Guid.NewGuid()}")
            .Options;

        _dbContext = new AppDbContext(options);
        _repository = new BookingRepository(_dbContext);

        // Generate test IDs
        _testUserId = _fixture.Create<Guid>();
        _testHotelId = _fixture.Create<Guid>();
        _testRoomCategoryId = _fixture.Create<Guid>();
        _testRoomId = _fixture.Create<Guid>();
    }

    public async Task InitializeAsync()
    {
        await SeedTestDataAsync();
    }

    public Task DisposeAsync()
    {
        _dbContext?.Dispose();
        return Task.CompletedTask;
    }

    private async Task SeedTestDataAsync()
    {
        var user = _fixture.Build<User>()
            .With(u => u.Id, _testUserId)
            .With(u => u.Email, "test.user@example.com")
            .With(u => u.Role, UserRole.User)
            .Create();

        var hotel = _fixture.Build<Hotel>()
            .With(h => h.Id, _testHotelId)
            .With(h => h.Name, "Grand Plaza Hotel")
            .With(h => h.StarRating, 5)
            .With(h => h.TotalRooms, 100)
            .With(h => h.HotelType, HotelType.Resort)
            .Without(h => h.Bookings)
            .Without(h => h.City)
            .Without(h => h.Gallery)
            .Without(h => h.Owner)
            .Create();


        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, _testRoomCategoryId)
            .With(rc => rc.HotelId, _testHotelId)
            .With(rc => rc.Name, "Deluxe Suite")
            .With(rc => rc.AdultsCapacity, 2)
            .With(rc => rc.ChildrenCapacity, 2)
            .With(rc => rc.PricePerNight, 250.00m)
            .With(rc => rc.RoomType, RoomType.Suite)
            .Create();

        var room = _fixture.Build<Room>()
            .With(r => r.Id, _testRoomId)
            .With(r => r.RoomNumber, "101")
            .With(r => r.RoomCategoryId, _testRoomCategoryId)
            .Create();

        var booking = _fixture.Build<Booking>()
            .With(b => b.UserId, _testUserId)
            .With(b => b.HotelId, _testHotelId)
            .With(b => b.CheckInDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7)))
            .With(b => b.CheckOutDate, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(9)))
            .With(b => b.GuestRemarks, "Early check-in requested")
            .With(b => b.PaymentDetails, _fixture.Build<PaymentDetails>()
                .With(pd => pd.Amount, 500.00m)
                .With(pd => pd.PaymentMethod, PaymentMethod.Card)
                .Create())
            .Create();

        booking.Rooms.Add(room);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Hotels.AddAsync(hotel);
        await _dbContext.RoomCategories.AddAsync(roomCategory);
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();
    }

    // Fixed BookingRepositoryTests.cs
    [Fact]
    public async Task AddAsync_ValidBooking_AddsToDatabase()
    {
        // Arrange
        var existingHotel = await _dbContext.Hotels.FirstAsync();
        var existingUser = await _dbContext.Users.FirstAsync();

        var newBooking = new Booking
        {
            Id = Guid.NewGuid(),
            UserId = existingUser.Id, // Use the actual user ID from seeded data
            HotelId = existingHotel.Id, // Use the actual hotel ID from seeded data
            CheckInDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(10)),
            CheckOutDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(12)),
            BookingDate = DateTime.UtcNow,
            GuestRemarks = "Test booking remarks",
            PaymentDetails = new PaymentDetails
            {
                Id = Guid.NewGuid(),
                Amount = 300.00m,
                PaymentNumber = 1,
                PaymentDate = DateTime.UtcNow,
                PaymentMethod = PaymentMethod.Card
            }
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
        savedBooking!.UserId.Should().Be(existingUser.Id); // Compare with the actual user ID
        savedBooking.HotelId.Should().Be(existingHotel.Id);
    }
    [Theory, AutoData]
    public async Task AddAsync_ValidBookingWithAutoData_AddsToDatabase(
        string guestRemarks,
        decimal paymentAmount,
        PaymentMethod paymentMethod)
    {
        // Arrange
        var newBooking = _fixture.Build<Booking>()
            .With(b => b.UserId, _testUserId)
            .With(b => b.HotelId, _testHotelId)
            .With(b => b.GuestRemarks, guestRemarks)
            .With(b => b.PaymentDetails, _fixture.Build<PaymentDetails>()
                .With(pd => pd.Amount, paymentAmount)
                .With(pd => pd.PaymentMethod, paymentMethod)
                .Create())
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
    public async Task AddAsync_NullBooking_ThrowsArgumentNullException()
    {
        // Arrange
        Booking nullBooking = null!;
        var cancellationToken = CancellationToken.None;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _repository.AddAsync(nullBooking, cancellationToken));
    }

    [Fact]
    public async Task GetByIdAsync_ExistingBooking_ReturnsBooking()
    {
        // Arrange
        var existingBooking = await _dbContext.Bookings.FirstAsync();
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByIdAsync(existingBooking.Id, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(existingBooking.Id);
        result.UserId.Should().Be(existingBooking.UserId);
        result.HotelId.Should().Be(existingBooking.HotelId);
        result.GuestRemarks.Should().Be(existingBooking.GuestRemarks);
        result.PaymentDetails.Should().NotBeNull();
        result.PaymentDetails.Amount.Should().Be(existingBooking.PaymentDetails.Amount);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingBooking_ReturnsNull()
    {
        // Arrange
        var nonExistingBookingId = _fixture.Create<Guid>();
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
        var existingBookingId = await _dbContext.Bookings.Select(b => b.Id).FirstAsync();
        var cancelledToken = new CancellationToken(canceled: true);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            _repository.GetByIdAsync(existingBookingId, cancelledToken));
    }

    [Fact]
    public async Task GetByIdAsync_EmptyGuid_ReturnsNull()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var cancellationToken = CancellationToken.None;

        // Act
        var result = await _repository.GetByIdAsync(emptyGuid, cancellationToken);

        // Assert
        result.Should().BeNull();
    }

    [Theory]
    [InlineData(PaymentMethod.Card)]
    [InlineData(PaymentMethod.Cash)]
    public async Task GetByIdAsync_BookingWithDifferentPaymentMethods_ReturnsCorrectPaymentDetails(
        PaymentMethod paymentMethod)
    {
        // Arrange
        var booking = _fixture.Build<Booking>()
            .With(b => b.UserId, _testUserId)
            .With(b => b.HotelId, _testHotelId)
            .With(b => b.PaymentDetails, _fixture.Build<PaymentDetails>()
                .With(pd => pd.PaymentMethod, paymentMethod)
                .Create())
            .Create();

        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

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
        var bookings = _fixture.Build<Booking>()
            .With(b => b.UserId, _testUserId)
            .With(b => b.HotelId, _testHotelId)
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

        var booking = _fixture.Build<Booking>()
            .With(b => b.UserId, _testUserId)
            .With(b => b.HotelId, _testHotelId)
            .With(b => b.CheckInDate, futureCheckIn)
            .With(b => b.CheckOutDate, futureCheckOut)
            .Create();

        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

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
*/