using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

public record GetDiscountByIdQuery(Guid HotelId, Guid RoomCategoryId, Guid DiscountId) : IRequest<Result<DiscountDto>>;