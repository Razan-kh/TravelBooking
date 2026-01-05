using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Commands;

public record DeleteDiscountCommand(Guid HotelId, Guid RoomCategoryId, Guid DiscountId) : IRequest<Result>;