using AutoFixture;
using AutoFixture.AutoMoq;
using Moq;
using TravelBooking.Application.Users.Services.Interfaces;
using TravelBooking.Application.Users.Handlers;

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