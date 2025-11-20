using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Xunit;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.Services;
using TravelBooking.Domain.Users;
using Microsoft.AspNetCore.Identity;
using TravelBooking.Application.Users.Services.Implementations;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Application.Users.Handlers;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Tests.Handlers.Authentication;

public class HotelSearchHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly LoginCommandHandler _sut;

    public HotelSearchHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _authServiceMock = _fixture.Freeze<Mock<IAuthService>>();
        _sut = new LoginCommandHandler(_authServiceMock.Object);
    }
}