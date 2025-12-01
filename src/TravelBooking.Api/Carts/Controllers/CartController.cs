using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Carts.Commands;
using TravelBooking.Application.Carts.Queries;
using Microsoft.AspNetCore.Authorization;

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

    [HttpPost]
    public async Task<IActionResult> AddRoomToCart([FromBody] AddRoomToCartCommand command)
    {
        var result = await _mediator.Send(command);
        return StatusCode(result.HttpStatusCode ?? 200, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetCart(CancellationToken ct)
    {
        _logger.LogInformation("GetCart endpoint called by user");
        
        var result = await _mediator.Send(new GetCartQuery(), ct);
        
        _logger.LogInformation("GetCart completed with status: {StatusCode}", 
            result.HttpStatusCode ?? 200);
            
        return StatusCode(result.HttpStatusCode ?? 200, result);
    }

    [HttpDelete("{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoveCartItemCommand(cartItemId), ct);
        return StatusCode(result.HttpStatusCode ?? 200, result);
    }
}