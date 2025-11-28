using AutoFixture;
using Moq;
using TravelBooking.Application.Carts.Services.Implementations;
using TravelBooking.Application.Carts.Services.Interfaces;
using TravelBooking.Tests.AddingToCart.TestHelpers;
using TravelBooking.Application.Carts.Mappers;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Application.Shared.Interfaces;

namespace TravelBooking.Tests.AddingToCart.Handlers;

public class CartServiceTests
{
    private readonly IFixture _fixture;

    private readonly Mock<IRoomAvailabilityService> _availability;
    private readonly Mock<ICartRepository> _repo;
    private readonly Mock<IUnitOfWork> _uow;
    private readonly Mock<ICartMapper> _mapper;

    private readonly CartService _service;

    public CartServiceTests()
    {
        _fixture = FixtureFactory.Create();

        // IMPORTANT: do NOT Freeze mocks with AutoFixture
        // Use manual mock creation and inject them instead.
        _availability = new Mock<IRoomAvailabilityService>();
        _repo = new Mock<ICartRepository>();
        _uow = new Mock<IUnitOfWork>();
        _mapper = new Mock<ICartMapper>();

        _fixture.Inject(_availability.Object);
        _fixture.Inject(_repo.Object);
        _fixture.Inject(_uow.Object);
        _fixture.Inject(_mapper.Object);

        _service = new CartService(
            _availability.Object,
            _repo.Object,
            _uow.Object,
            _mapper.Object);
    }

    [Fact]
    public async Task AddRoomToCart_ShouldFail_WhenNotAvailable()
    {
        _availability
            .Setup(x => x.HasAvailableRoomsAsync(
                It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _service.AddRoomToCartAsync(
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            1, default);

        Assert.False(result.IsSuccess);
        Assert.Equal("Not enough rooms available for the selected period.", result.Error);
    }

    [Fact]
    public async Task AddRoomToCart_ShouldCreateNewCart_WhenCartNotFound()
    {
        _repo.Setup(x => x.GetUserCartAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((Cart)null);

        _availability.Setup(x => x.HasAvailableRoomsAsync(
                It.IsAny<Guid>(), It.IsAny<DateOnly>(), It.IsAny<DateOnly>(),
                It.IsAny<int>(), default))
            .ReturnsAsync(true);

        var result = await _service.AddRoomToCartAsync(
            Guid.NewGuid(), Guid.NewGuid(),
            DateOnly.FromDateTime(DateTime.Today),
            DateOnly.FromDateTime(DateTime.Today.AddDays(1)),
            1, default);

        Assert.True(result.IsSuccess);

        _repo.Verify(x => x.AddOrUpdateAsync(It.Is<Cart>(c => c.Items.Any())), Times.Once);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldFail_WhenItemNotFound()
    {
        _repo.Setup(x => x.GetCartItemByIdAsync(It.IsAny<Guid>(), default))
            .ReturnsAsync((CartItem)null);

        var result = await _service.RemoveItemAsync(Guid.NewGuid(), default);

        Assert.False(result.IsSuccess);
        Assert.Equal("Cart item not found.", result.Error);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldRemoveItem_WhenItemExists()
    {
        var item = _fixture.Create<CartItem>();

        _repo.Setup(x => x.GetCartItemByIdAsync(item.Id, default))
            .ReturnsAsync(item);

        var result = await _service.RemoveItemAsync(item.Id, default);

        Assert.True(result.IsSuccess);
        _repo.Verify(x => x.RemoveItem(item), Times.Once);
        _uow.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}
