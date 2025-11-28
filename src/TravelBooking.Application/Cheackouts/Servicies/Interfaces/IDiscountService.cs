using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IDiscountService
{
    decimal CalculateTotal(IEnumerable<CartItem> items);
}