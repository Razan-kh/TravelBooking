using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Users.Services.Implementations;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Application.Interfaces.Security;

namespace TravelBooking.Tests.Services.Authentication;

public class AuthServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtService> _jwtServiceMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());

        _userRepoMock = _fixture.Freeze<Mock<IUserRepository>>();
        _passwordHasherMock = _fixture.Freeze<Mock<IPasswordHasher>>();
        _jwtServiceMock = _fixture.Freeze<Mock<IJwtService>>();

        _sut = new AuthService(
            _userRepoMock.Object,
            _passwordHasherMock.Object,
            _jwtServiceMock.Object
        );
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsSuccessResultWithToken()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.PasswordHash, "hashedPassword")
            .Without(u => u.Bookings)
            .Create();
        var email = user.Email;
        var password = "plainPassword";
        var token = "jwt_token";

        _userRepoMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.Verify(user.PasswordHash, password))
            .Returns(true);

        _jwtServiceMock.Setup(x => x.CreateToken(user))
            .Returns(token);

        // Act
        var result = await _sut.LoginAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.AccessToken.Should().Be(token);
        result.Value.TokenType.Should().Be("Bearer");

        _jwtServiceMock.Verify(x => x.CreateToken(user), Times.Once);
        _passwordHasherMock.Verify(x => x.Verify(user.PasswordHash, password), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ReturnsFailureResult()
    {
        // Arrange
        var email = "nonexistent@example.com";
        var password = "anyPassword";

        _userRepoMock.Setup(x => x.GetByEmailAsync(email))
            .ReturnsAsync(default(User?));

        // Act
        var result = await _sut.LoginAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        // FIX: Check for the actual error message your service returns
        result.Error.Should().Be("Invalid credentials");

        _jwtServiceMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ReturnsFailureResult()
    {
        // Arrange
        var user = _fixture.Build<User>()
            .With(u => u.PasswordHash, "hashedPassword")
            .Without(u => u.Bookings)
            .Create();
        var password = "wrongPassword";

        _userRepoMock.Setup(x => x.GetByEmailAsync(user.Email))
            .ReturnsAsync(user);

        // FIX: Use correct method name and parameter order
        _passwordHasherMock.Setup(x => x.Verify(user.PasswordHash, password))
            .Returns(false);

        // Act
        var result = await _sut.LoginAsync(user.Email, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        // FIX: Check for the actual error message your service returns
        result.Error.Should().Be("Invalid credentials");

        _jwtServiceMock.Verify(x => x.CreateToken(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenRepositoryThrows_ReturnsFailureResultWithSystemError()
    {
        // Arrange
        var email = "test@example.com";
        var password = "anyPassword";

        _userRepoMock.Setup(x => x.GetByEmailAsync(email))
            .ThrowsAsync(new Exception("DB failure"));

        // Act
        var result = await _sut.LoginAsync(email, password);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Invalid credentials");
    }
}