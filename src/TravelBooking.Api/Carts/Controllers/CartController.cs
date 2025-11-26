using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.AddingToCart.Queries;
using Microsoft.AspNetCore.Authorization;

namespace TravelBooking.Api.Carts.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<IActionResult> AddRoomToCart([FromBody] AddRoomToCartCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.HttpStatusCode ?? 200, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCartQuery(), ct);
        return StatusCode(result.HttpStatusCode ?? 200, result);
    }

    [HttpDelete("{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoveCartItemCommand(cartItemId), ct);
        return StatusCode(result.HttpStatusCode ?? 200, result);
    }
}