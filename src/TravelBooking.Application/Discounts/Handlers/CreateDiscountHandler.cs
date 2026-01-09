using MediatR;
using TravelBooking.Application.Discounts.Commands;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Discounts.Handlers;

public class CreateDiscountHandler : IRequestHandler<CreateDiscountCommand, Result<DiscountDto>>
{
    private readonly IDiscountService _service;
    public CreateDiscountHandler(IDiscountService service) => _service = service;
    public Task<Result<DiscountDto>> Handle(CreateDiscountCommand request, CancellationToken cancellationToken)
    => _service.CreateAsync(request.HotelId, request.RoomCategoryId, request.Dto, cancellationToken);
}