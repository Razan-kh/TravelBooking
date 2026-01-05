using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Handlers;

public class GetDiscountsHandler : IRequestHandler<GetDiscountsQuery, Result<IEnumerable<DiscountDto>>>
{
    private readonly IDiscountService _service;
    public GetDiscountsHandler(IDiscountService service) => _service = service;
    public Task<Result<IEnumerable<DiscountDto>>> Handle(GetDiscountsQuery request, CancellationToken cancellationToken)
    => _service.GetAllAsync(request.HotelId, request.RoomCategoryId, cancellationToken);
}