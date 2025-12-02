
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Bookings.Commands;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Tests.Application.Checkout.Utils;

namespace TravelBooking.Tests.Application.Bookings.Handlers;

public class CheckoutHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<ICartService> _cartServiceMock;
    private readonly Mock<IPaymentService> _paymentServiceMock;
    private readonly Mock<IBookingService> _bookingServiceMock;
    private readonly Mock<IPdfService> _pdfServiceMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly CheckoutHandler _sut;

    public CheckoutHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        // Handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _cartServiceMock = _fixture.Freeze<Mock<ICartService>>();
        _paymentServiceMock = _fixture.Freeze<Mock<IPaymentService>>();
        _bookingServiceMock = _fixture.Freeze<Mock<IBookingService>>();
        _pdfServiceMock = _fixture.Freeze<Mock<IPdfService>>();
        _emailServiceMock = _fixture.Freeze<Mock<IEmailService>>();
        _userRepositoryMock = _fixture.Freeze<Mock<IUserRepository>>();
        _unitOfWorkMock = _fixture.Freeze<Mock<IUnitOfWork>>();

        _sut = new CheckoutHandler(
            _cartServiceMock.Object,
            _paymentServiceMock.Object,
            _bookingServiceMock.Object,
            _pdfServiceMock.Object,
            _emailServiceMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_EmptyCart_ReturnsFailureResult()
    {
        // Arrange
        var command = _fixture.Create<CheckoutCommand>();
        _cartServiceMock.Setup(c => c.GetUserCartAsync(command.UserId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync((Cart)null);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cart is empty.");
    }

    [Fact]
    public async Task Handle_PaymentFails_ReturnsFailureResult()
    {
        // Arrange
        var command = _fixture.Create<CheckoutCommand>();
        var cart = _fixture.CreateTestCart();
        _cartServiceMock.Setup(c => c.GetUserCartAsync(command.UserId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(cart);
        _paymentServiceMock.Setup(p => p.ProcessPaymentAsync(command.UserId, command.PaymentMethod, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(Result.Failure("Card declined"));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Card declined");
    }

    [Fact]
    public async Task Handle_ValidCartAndPayment_CreatesBookingsAndSendsEmails()
    {
        // Arrange
        var command = _fixture.Create<CheckoutCommand>();
        var cart = _fixture.CreateTestCart();
        var user = _fixture.CreateValidUser();
        var bookings = _fixture.CreateValidBookings();

        _cartServiceMock.Setup(c => c.GetUserCartAsync(command.UserId, It.IsAny<CancellationToken>()))
                        .ReturnsAsync(cart);
        _paymentServiceMock.Setup(p => p.ProcessPaymentAsync(command.UserId, command.PaymentMethod, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(Result.Success());
        _bookingServiceMock.Setup(b => b.CreateBookingsAsync(cart, command, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(bookings);
        _userRepositoryMock.Setup(u => u.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
                           .ReturnsAsync(user);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        _pdfServiceMock.Verify(p => p.GenerateInvoice(It.IsAny<Booking>()), Times.Exactly(bookings.Count));
        _emailServiceMock.Verify(e => e.SendBookingConfirmationAsync(
            user.Email,
            It.IsAny<Booking>(),
            It.IsAny<byte[]>(),
            It.IsAny<CancellationToken>()),
        Times.Exactly(bookings.Count));
        _cartServiceMock.Verify(c => c.ClearCartAsync(command.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }
}