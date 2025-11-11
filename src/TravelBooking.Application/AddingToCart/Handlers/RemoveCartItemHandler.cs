
using MediatR;
using TravelBooking.Application.AddingToCar.Commands;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Repositories;

namespace TravelBooking.Application.AddingToCart.Handlers;

public class RemoveCartItemHandler : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly ICartRepository _cartRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveCartItemHandler(ICartRepository cartRepository, IUnitOfWork unitOfWork)
    {
        _cartRepository = cartRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var item = await _cartRepository.GetCartItemByIdAsync(request.CartItemId);
        if (item == null)
            return Result.Failure("Cart item not found.", "NOT_FOUND", 404);

        _cartRepository.RemoveItem(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}