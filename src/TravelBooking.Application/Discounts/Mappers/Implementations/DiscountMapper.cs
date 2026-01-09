using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Discounts.Mappers.Interfaces;
using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Application.Discounts.Mappers.Implementations;

[Mapper]
public partial class DiscountMapper : IDiscountMapper
{
    public partial DiscountDto ToDto(Discount discount);
}