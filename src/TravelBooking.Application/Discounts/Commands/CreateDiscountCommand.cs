using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Commands;

public record CreateDiscountCommand(Guid HotelId, Guid RoomCategoryId, CreateDiscountDto Dto) : IRequest<Result<DiscountDto>>;