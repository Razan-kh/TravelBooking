using MediatR;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Handlers;

public class GetDiscountByIdHandler : IRequestHandler<GetDiscountByIdQuery, Result<DiscountDto>>
{
    private readonly IDiscountService _service;
    public GetDiscountByIdHandler(IDiscountService service) => _service = service;
    public Task<Result<DiscountDto>> Handle(GetDiscountByIdQuery request, CancellationToken cancellationToken)
    => _service.GetByIdAsync(request.HotelId, request.RoomCategoryId, request.DiscountId, cancellationToken);
}