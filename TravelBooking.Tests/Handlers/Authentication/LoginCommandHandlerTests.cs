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

public class LoginCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly LoginCommandHandler _sut;

    public LoginCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _authServiceMock = _fixture.Freeze<Mock<IAuthService>>();
        _sut = new LoginCommandHandler(_authServiceMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ReturnsSuccessResult()
    {
        // Arrange
        var command = _fixture.Build<LoginCommand>()
            .With(x => x.Email, "test@example.com")
            .With(x => x.Password, "Password123!")
            .Create();

        var loginResponse = new LoginResponseDto
        {
            AccessToken = "jwt_token"
        };

        _authServiceMock
            .Setup(x => x.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(Result.Success(loginResponse));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be("jwt_token");
        _authServiceMock.Verify(x => x.LoginAsync(command.Email, command.Password), Times.Once);
    }

    [Fact]
    public async Task Handle_WithInvalidCredentials_ReturnsFailureResult()
    {
        // Arrange
        var command = _fixture.Build<LoginCommand>()
            .With(x => x.Email, "wrong@example.com")
            .With(x => x.Password, "WrongPassword")
            .Create();

        _authServiceMock
            .Setup(x => x.LoginAsync(command.Email, command.Password))
            .ReturnsAsync(Result.Failure<LoginResponseDto>("Invalid credentials", "INVALID_CREDENTIALS", 401));

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorCode.Should().Be("INVALID_CREDENTIALS");
        result.HttpStatusCode.Should().Be(401);
        _authServiceMock.Verify(x => x.LoginAsync(command.Email, command.Password), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAuthServiceThrows_ReturnsFailureResult()
    {
        // Arrange
        var command = _fixture.Create<LoginCommand>();

        _authServiceMock
            .Setup(x => x.LoginAsync(command.Email, command.Password))
            .ThrowsAsync(new System.Exception("Unexpected error"));

        // Act
        var act = async () => await _sut.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<System.Exception>()
            .WithMessage("Unexpected error");
    }
}