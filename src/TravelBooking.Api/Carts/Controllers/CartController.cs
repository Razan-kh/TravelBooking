using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Carts.Commands;
using TravelBooking.Application.Carts.Queries;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Api.Extensions;
using TravelBooking.Application.Carts.DTOs;

namespace TravelBooking.Api.Carts.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly IMediator _mediator;

    public CartController(IMediator mediator, ILogger<CartController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddRoomToCart([FromBody] AddRoomToCartCommand command)
    {
        var result = await _mediator.Send(command);
        return result.ToActionResult();
    }

    [HttpGet]
    public async Task<ActionResult<List<CartItemDto>>> GetCart(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCartQuery(), ct);
        if (result.IsSuccess)
            return Ok(result.Value);

        return result.ToActionResult();
    }

    [HttpDelete("items/{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoveCartItemCommand(cartItemId), ct);
        if (result.IsSuccess)
            return new NoContentResult();

        return result.ToActionResult();
    }
}