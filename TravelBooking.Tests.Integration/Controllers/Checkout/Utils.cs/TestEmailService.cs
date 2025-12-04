
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Factories;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
using System.Data;
using TravelBooking.Domain.Cities.Entities;

namespace BookingSystem.IntegrationTests.Checkout.Utils;


/// <summary>
/// Test email service that captures emails instead of sending them
/// </summary>
public class TestEmailService : IEmailService
{
    public List<EmailCapture> SentEmails { get; } = new();

    public Task SendBookingConfirmationAsync(
        string email,
        Booking booking,
        byte[]? pdfInvoice,
        CancellationToken ct = default)
    {
        SentEmails.Add(new EmailCapture(email, booking, pdfInvoice));
        return Task.CompletedTask;
    }

    public record EmailCapture(string Email, Booking Booking, byte[]? PdfInvoice);
}