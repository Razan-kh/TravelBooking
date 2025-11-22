using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.AddingToCart.Queries;

namespace TravelBooking.Api.Carts.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartController> _logger;

    public CartController(IMediator mediator, ILogger<CartController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Adds a room to the user’s cart.
    /// </summary>
    /// <remarks>
    /// Example:
    /// 
    ///     POST /api/cart
    ///     {
    ///         "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "roomCategoryId": "a2345d56-8890-4b7a-8234-19db8d88432e",
    ///         "checkIn": "2025-11-15",
    ///         "checkOut": "2025-11-18",
    ///         "quantity": 2
    ///     }
    /// </remarks>
    /// <param name="command">The command containing cart details.</param>
    /// <returns>Result of adding the room to the cart.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AddRoomToCart([FromBody] AddRoomToCartCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(Result.Failure("Invalid request data."));

        var result = await _mediator.Send(command);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to add room to cart: {Error}", result.Error);
            return StatusCode(result.HttpStatusCode ?? 400, result);
        }

        _logger.LogInformation("Room successfully added to cart for User {UserId}", command.UserId);
        return Ok(result);
    }

    /// <summary>
    /// Get all cart items for a specific user.
    /// </summary>
    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetCart(Guid userId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCartQuery(userId), ct);

        if (!result.IsSuccess)
            return StatusCode(result.HttpStatusCode ?? 400, result);

        return Ok(result);
    }

    /// <summary>
    /// Remove a specific item from the cart.
    /// </summary>
    [HttpDelete("{cartItemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid cartItemId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoveCartItemCommand(cartItemId), ct);

        if (!result.IsSuccess)
            return StatusCode(result.HttpStatusCode ?? 400, result);

        return Ok(result);
    }
}