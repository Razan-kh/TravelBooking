using MediatR;
using TravelBooking.Application.Discounts.Commands;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Handlers;

public class DeleteDiscountHandler : IRequestHandler<DeleteDiscountCommand, Result>
{
private readonly IDiscountService _service;
public DeleteDiscountHandler(IDiscountService service) => _service = service;
public Task<Result> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
=> _service.DeleteAsync(request.HotelId, request.RoomCategoryId, request.DiscountId, cancellationToken);
}