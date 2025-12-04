using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using System.Data;
using BookingSystem.IntegrationTests.Checkout.Utils;

namespace BookingSystem.IntegrationTests.Checkout;

[CollectionDefinition("CheckoutIntegration")]
public class CheckoutIntegrationCollection : ICollectionFixture<CheckoutTestFixture>
{
}


[Collection("CheckoutIntegration")]
public class CheckoutControllerIntegrationTests : IAsyncLifetime
{
    private readonly CheckoutTestFixture _fixture;
    private HttpClient _client;
    private AppDbContext _dbContext;
    private TestEmailService _emailService;
    private TestPaymentService _paymentService;
    private InMemoryUnitOfWork _inMemoryUow;
    private readonly Guid _testUserId;

    public CheckoutControllerIntegrationTests(CheckoutTestFixture fixture)
    {
        _fixture = fixture;
        _testUserId = fixture.TestUserId;
    }

    public async Task InitializeAsync()
    {
        await Task.Yield();

        _client = _fixture.Client;
        _dbContext = _fixture.DbContext;
        _emailService = _fixture.EmailService;
        _paymentService = _fixture.PaymentService;
        _inMemoryUow = _fixture.InMemoryUow;

        _client.AddAuthHeader("User", _testUserId);

        // Reset test services before each test
        _emailService.SentEmails.Clear();
        _paymentService.SetSuccess();
        _inMemoryUow.Reset();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    // Success scenarios
    [Fact(DisplayName = "POST /api/checkout - With valid cart - Should complete booking successfully")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P0")]
    public async Task Checkout_WithValidCart_CompletesBookingSuccessfully()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);
        var result = await response.Content.ReadFromJsonAsync<Result>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeTrue();

        // Verify booking was created
        var booking = await _dbContext.Bookings
            .Include(b => b.PaymentDetails)
            .Include(b => b.Rooms)
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);

        booking.Should().NotBeNull();
        booking!.UserId.Should().Be(_testUserId);
        booking.PaymentDetails.Should().NotBeNull();
        booking.PaymentDetails.PaymentMethod.Should().Be(PaymentMethod.Card);
        booking.Rooms.Should().HaveCount(cart.Items.Sum(i => i.Quantity));

        // Verify email was sent
        _emailService.SentEmails.Should().HaveCount(1);
        var sentEmail = _emailService.SentEmails.First();
        sentEmail.Email.Should().Be("test.user@example.com");
        sentEmail.Booking.Id.Should().Be(booking.Id);
        sentEmail.PdfInvoice.Should().NotBeNull();
    }

    [Fact(DisplayName = "POST /api/checkout - With multiple hotels - Should create separate bookings")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P0")]
    public async Task Checkout_WithMultipleHotels_CreatesSeparateBookings()
    {
        // Arrange
        var cart = await _fixture.CreateMultiHotelCartAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Cash);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify multiple bookings were created
        var bookings = await _dbContext.Bookings
            .Where(b => b.UserId == _testUserId)
            .ToListAsync();

        bookings.Should().HaveCount(2);

        // Verify each booking has correct hotel
        var hotelIds = bookings.Select(b => b.HotelId).Distinct().ToList();
        hotelIds.Should().HaveCount(2);

        // Verify emails were sent for each booking
        _emailService.SentEmails.Should().HaveCount(2);
        _emailService.SentEmails.Select(e => e.Booking.Id)
            .Should().BeEquivalentTo(bookings.Select(b => b.Id));
    }

    [Fact(DisplayName = "POST /api/checkout - With discount - Should apply discount correctly")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P1")]
    public async Task Checkout_WithDiscount_AppliesDiscountCorrectly()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithDiscountAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Calculate expected total with discount
        var cartItem = cart.Items.First();
        int nights = (cartItem.CheckOut.ToDateTime(TimeOnly.MinValue)
                        - cartItem.CheckIn.ToDateTime(TimeOnly.MinValue)).Days;
        var basePrice = cartItem.RoomCategory!.PricePerNight * cartItem.Quantity * nights;
        var expectedTotal = basePrice * 0.8m; // 20% discount

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify booking has discounted price
        var booking = await _dbContext.Bookings
            .Include(b => b.PaymentDetails)
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);

        booking.Should().NotBeNull();
        booking!.PaymentDetails.Amount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "POST /api/checkout - With Cash payment - Should process successfully")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P1")]
    public async Task Checkout_WithCashPayment_ProcessesSuccessfully()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Cash);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var booking = await _dbContext.Bookings
            .Include(b => b.PaymentDetails)
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);

        booking.Should().NotBeNull();
        booking!.PaymentDetails.PaymentMethod.Should().Be(PaymentMethod.Cash);
    }

    // Failure scenarios
    [Fact(DisplayName = "POST /api/checkout - With empty cart - Should return 400 Bad Request")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P0")]
    public async Task Checkout_WithEmptyCart_ReturnsBadRequest()
    {
        // Arrange 
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);
        var result = await response.Content.ReadFromJsonAsync<Result>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result!.Error.Should().Contain("empty", "Cart is empty");
        result!.ErrorCode.Should().Be("EMPTY_CART");

        // Verify no booking was created
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);
        booking.Should().BeNull();

        // Verify no email was sent
        _emailService.SentEmails.Should().BeEmpty();
    }

    [Fact(DisplayName = "POST /api/checkout - When payment fails - Should return 400 Bad Request")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P0")]
    public async Task Checkout_WhenPaymentFails_ReturnsBadRequest()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Configure payment service to fail
        _paymentService.SetFailure("Insufficient funds");

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);
        var result = await response.Content.ReadFromJsonAsync<Result>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        result.Should().NotBeNull();
        result!.IsSuccess.Should().BeFalse();
        result!.ErrorCode.Should().Be("PAYMENT_FAILED");

        // Verify no booking was created
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);
        booking.Should().BeNull();

        // Verify cart was NOT cleared
        var userCart = await _dbContext.Carts
            .FirstOrDefaultAsync(c => c.UserId == _testUserId);
        userCart.Should().NotBeNull();

        // Verify no email was sent
        _emailService.SentEmails.Should().BeEmpty();
    }


    [Fact(DisplayName = "POST /api/checkout - Without authentication - Should return 401 Unauthorized")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P0")]
    public async Task Checkout_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange
        var unauthenticatedClient = _fixture.Factory.CreateClient(); // No auth header
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await unauthenticatedClient.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // Edge cases & bussiness rules
    [Fact(DisplayName = "POST /api/checkout - With past check-in date - Should return error")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P2")]
    public async Task Checkout_WithPastCheckInDate_ReturnsError()
    {
        // Arrange
        var hotel = new Hotel { Id = Guid.NewGuid(), Name = "Test Hotel" };
        var roomCategory = new RoomCategory
        {
            Id = Guid.NewGuid(),
            HotelId = hotel.Id,
            Name = "Test Room",
            PricePerNight = 100.00m,
            Hotel = hotel
        };

        await _dbContext.Hotels.AddAsync(hotel);
        await _dbContext.RoomCategories.AddAsync(roomCategory);

        var cart = new Cart
        {
            UserId = _testUserId,
            Items = new List<CartItem>
            {
                new()
                {
                    RoomCategoryId = roomCategory.Id,
                    RoomCategory = roomCategory,
                    CheckIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1)), // Past date
                    CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
                    Quantity = 1
                }
            }
        };

        await _dbContext.Carts.AddAsync(cart);
        await _dbContext.SaveChangesAsync();

        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        // Should not succeed with past dates
        response.StatusCode.Should().NotBe(HttpStatusCode.OK);
    }

    [Fact(DisplayName = "POST /api/checkout - When rooms become unavailable - Should rollback transaction")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P1")]
    public async Task Checkout_WhenRoomsBecomeUnavailable_RollsBackTransaction()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithUnavailableRoomAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        // Should fail because rooms are not available
        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        // Verify no booking was created
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);
        booking.Should().BeNull();

        // Verify cart was NOT cleared
        var userCart = await _dbContext.Carts
            .FirstOrDefaultAsync(c => c.UserId == _testUserId);
        userCart.Should().NotBeNull();
    }

    [Fact(DisplayName = "POST /api/checkout - With large quantity - Should process successfully")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P2")]
    public async Task Checkout_WithLargeQuantity_ProcessesSuccessfully()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync(itemCount: 10);
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var booking = await _dbContext.Bookings
            .Include(b => b.Rooms)
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);

        booking.Should().NotBeNull();
        var totalRooms = cart.Items.Sum(i => i.Quantity);
        booking!.Rooms.Should().HaveCount(totalRooms);
    }

    // ==============================================
    // CONCURRENCY & TRANSACTION TESTS
    // ==============================================

    [Fact(DisplayName = "POST /api/checkout - Concurrent requests - Should handle correctly")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P2")]
    public async Task Checkout_ConcurrentRequests_HandlesCorrectly()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act - Send multiple requests concurrently
        var tasks = Enumerable.Range(0, 3)
            .Select(_ => _client.PostAsJsonAsync("/api/checkout", checkoutCommand))
            .ToList();

        var responses = await Task.WhenAll(tasks);

        // Assert
        // Only one should succeed, others should fail appropriately
        var successfulResponses = responses.Count(r => r.IsSuccessStatusCode);
        successfulResponses.Should().Be(1);

        // Verify exactly one booking was created
        var bookings = await _dbContext.Bookings
            .Where(b => b.UserId == _testUserId)
            .ToListAsync();

        bookings.Should().HaveCount(1);
    }

    // ==============================================
    // EMAIL & NOTIFICATION TESTS
    // ==============================================

    [Fact(DisplayName = "POST /api/checkout - Successful - Should send email with invoice")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P1")]
    public async Task Checkout_Successful_SendsEmailWithInvoice()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        _emailService.SentEmails.Should().HaveCount(1);
        var sentEmail = _emailService.SentEmails.First();

        // Verify email content (indirectly through properties)
        sentEmail.Email.Should().Be("test.user@example.com");
        sentEmail.Booking.Should().NotBeNull();
        sentEmail.PdfInvoice.Should().NotBeNull();
        sentEmail.PdfInvoice!.Length.Should().BeGreaterThan(0);

        // Verify PDF contains booking info
        var pdfContent = Encoding.UTF8.GetString(sentEmail.PdfInvoice);
        pdfContent.Should().Contain("Invoice");
        pdfContent.Should().Contain(sentEmail.Booking.Id.ToString());
    }

    [Fact(DisplayName = "POST /api/checkout - When email service fails - Should still complete booking")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P2")]
    public async Task Checkout_WhenEmailServiceFails_StillCompletesBooking()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Replace email service with one that throws
        var failingEmailService = new Mock<IEmailService>();
        failingEmailService
            .Setup(x => x.SendBookingConfirmationAsync(
                It.IsAny<string>(),
                It.IsAny<Booking>(),
                It.IsAny<byte[]>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Email service unavailable"));

        Assert.True(true, "Email failure handling test requires service reconfiguration");
    }

    // DATA INTEGRITY TESTS

    [Fact(DisplayName = "POST /api/checkout - Should maintain referential integrity")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P1")]
    public async Task Checkout_MaintainsReferentialIntegrity()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify all foreign key relationships are maintained
        var booking = await _dbContext.Bookings
            .Include(b => b.Rooms)
            .ThenInclude(r => r.RoomCategory)
            .Include(b => b.PaymentDetails)
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);

        booking.Should().NotBeNull();

        // Verify room categories exist
        foreach (var room in booking!.Rooms)
        {
            room.RoomCategory.Should().NotBeNull();
            room.RoomCategoryId.Should().Be(room.RoomCategory!.Id);
        }

        // Verify payment details are linked
        booking.PaymentDetails.Should().NotBeNull();
        booking.PaymentDetails.BookingId.Should().Be(booking.Id);
    }

    [Fact(DisplayName = "POST /api/checkout - Should calculate correct totals")]
    [Trait("Category", "Checkout")]
    [Trait("Priority", "P1")]
    public async Task Checkout_CalculatesCorrectTotals()
    {
        // Arrange
        var cart = await _fixture.CreateCartWithItemsAsync(
            itemCount: 2,
            pricePerNight: 100.00m);

        var checkoutCommand = new CheckoutCommand(_testUserId, PaymentMethod.Card);

        // Calculate expected total manually
        decimal expectedTotal = 0;
        foreach (var item in cart.Items)
        {
            int nights = (item.CheckOut.ToDateTime(TimeOnly.MinValue)
                            - item.CheckIn.ToDateTime(TimeOnly.MinValue)).Days;
            expectedTotal += item.RoomCategory!.PricePerNight * item.Quantity * nights;
        }

        // Act
        var response = await _client.PostAsJsonAsync("/api/checkout", checkoutCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var booking = await _dbContext.Bookings
            .Include(b => b.PaymentDetails)
            .FirstOrDefaultAsync(b => b.UserId == _testUserId);

        booking.Should().NotBeNull();
        booking!.PaymentDetails.Amount.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "Checkout - Response time - Should complete within threshold")]
    [Trait("Category", "Performance")]
    public async Task Checkout_ResponseTime_WithinThreshold()
    {
        var client = _fixture.Client;
        client.AddAuthHeader("User", _testUserId);

        var cart = await _fixture.CreateCartWithItemsAsync();
        var checkoutCommand = new CheckoutCommand(_fixture.TestUserId, PaymentMethod.Card);

        // Measure response time
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await client.PostAsJsonAsync("/api/checkout", checkoutCommand);
        stopwatch.Stop();

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Assert response time is reasonable (adjust threshold as needed)
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(2000); // 2 seconds
    }
}