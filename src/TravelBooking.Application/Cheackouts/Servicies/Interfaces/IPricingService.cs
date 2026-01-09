using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.Cheackout.Servicies.Interfaces;

public interface IPricingService
{
    decimal CalculateTotal(IEnumerable<CartItem> items);
}