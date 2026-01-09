using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Application.Discounts.Mappers.Interfaces;

public interface IDiscountMapper
{
    DiscountDto ToDto(Discount discount);
}