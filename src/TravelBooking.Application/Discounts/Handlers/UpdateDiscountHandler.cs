using MediatR;
using TravelBooking.Application.Discounts.Commands;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Handlers;

public class UpdateDiscountHandler : IRequestHandler<UpdateDiscountCommand, Result<DiscountDto>>
{
    private readonly IDiscountService _service;
    public UpdateDiscountHandler(IDiscountService service) => _service = service;
    public Task<Result<DiscountDto>> Handle(UpdateDiscountCommand request, CancellationToken cancellationToken)
    => _service.UpdateAsync(request.HotelId, request.RoomCategoryId, request.Dto, cancellationToken);
}