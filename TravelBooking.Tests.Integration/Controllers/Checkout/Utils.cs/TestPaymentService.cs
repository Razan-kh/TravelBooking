
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

namespace BookingSystem.IntegrationTests.Checkout;

/// <summary>
/// Test payment service for integration testing
/// </summary>
public class TestPaymentService : IPaymentService
{
    private bool _shouldSucceed = true;
    private string? _errorMessage;

    public void SetFailure(string errorMessage)
    {
        _shouldSucceed = false;
        _errorMessage = errorMessage;
    }

    public void SetSuccess()
    {
        _shouldSucceed = true;
        _errorMessage = null;
    }

    public Task<Result> ProcessPaymentAsync(
        Guid userId,
        PaymentMethod paymentMethod,
        CancellationToken ct = default)
    {
        if (!_shouldSucceed)
        {
            return Task.FromResult(Result.Failure(
                _errorMessage ?? "Payment failed",
                "PAYMENT_FAILED"));
        }

        return Task.FromResult(Result.Success());
    }
}