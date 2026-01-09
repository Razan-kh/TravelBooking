using TravelBooking.Tests.Carts.TestHelpers;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using TravelBooking.Application.Cheackout.Servicies.Implementations;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Tests.Unit.Cheackout.Servicies;

public class DiscountServiceTests
{
    private readonly IFixture _fixture;
    private readonly DiscountService _sut;

    public DiscountServiceTests()
    {
        //  _fixture = new Fixture();
        _fixture = new Fixture().Customize(new AutoMoqCustomization { ConfigureMembers = true });

        // Add this to handle circular references
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _sut = new DiscountService();
    }

    [Fact]
    public void CalculateTotal_WithDiscounts_ReturnsCorrectTotal()
    {
        // Arrange
        var roomCategory = _fixture.CreateRoomCategoryWithDiscount();

        var item = new CartItem
        {
            RoomCategory = roomCategory,
            Quantity = 2,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var total = _sut.CalculateTotal(new List<CartItem> { item });

        // Assert
        total.Should().Be(180); // 2 * 100 * 0.9
    }

    [Fact]
    public void CalculateTotal_WithoutDiscounts_ReturnsFullTotal()
    {
        // Arrange
        var roomCategory = _fixture.CreateRoomCategoryWithoutDiscounts(50.0m);

        var item = new CartItem
        {
            RoomCategory = roomCategory,
            Quantity = 3,
            CheckIn = DateOnly.FromDateTime(DateTime.UtcNow),
            CheckOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1))
        };

        // Act
        var total = _sut.CalculateTotal(new List<CartItem> { item });

        // Assert
        total.Should().Be(150); // 3 * 50
    }
}