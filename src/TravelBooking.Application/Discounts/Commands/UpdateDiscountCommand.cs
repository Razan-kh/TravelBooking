using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Commands;

public record UpdateDiscountCommand(Guid HotelId, Guid RoomCategoryId, UpdateDiscountDto Dto) : IRequest<Result<DiscountDto>>;