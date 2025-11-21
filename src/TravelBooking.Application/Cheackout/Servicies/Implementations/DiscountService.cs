using TravelBooking.Application.Cheackout.Servicies.Interfaces;
using TravelBooking.Domain.Carts.Entities;

namespace TravelBooking.Application.Cheackout.Servicies.Implementations;

public class DiscountService : IDiscountService
{
    public decimal CalculateTotal(IEnumerable<CartItem> items)
    {
        decimal total = 0;
        foreach (var item in items)
        {
            decimal price = item.RoomCategory.PricePerNight * item.Quantity;

            var discount = item.RoomCategory.Discounts
                .FirstOrDefault(d => d.StartDate <= item.CheckIn.ToDateTime(TimeOnly.MinValue)
                                  && d.EndDate >= item.CheckOut.ToDateTime(TimeOnly.MinValue));

            if (discount != null)
                price -= price * (discount.DiscountPercentage / 100m);

            total += price;
        }

        return total;
    }
}