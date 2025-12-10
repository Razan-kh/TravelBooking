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
            int nights = (item.CheckOut.ToDateTime(TimeOnly.MinValue)
                        - item.CheckIn.ToDateTime(TimeOnly.MinValue)).Days;

            decimal price = item.RoomCategory.PricePerNight * item.Quantity * nights;

            var discount = item.RoomCategory.Discounts
                .FirstOrDefault(d => d.StartDate <= item.CheckIn.ToDateTime(TimeOnly.MinValue)
                                  && d.EndDate >= item.CheckOut.ToDateTime(TimeOnly.MinValue));

            if (discount is not null)
                price -= price * (discount.DiscountPercentage / 100m);

            total += price;
        }

        return total;
    }
}