using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

public record GetDiscountsQuery(Guid HotelId, Guid RoomCategoryId) : IRequest<Result<IEnumerable<DiscountDto>>>;