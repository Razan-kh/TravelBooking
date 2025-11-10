using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Bookings.Commands;
using TravelBooking.Domain.Shared;

namespace TravelBooking.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Adds a room to the user's cart.
        /// </summary>
        /// <param name="command">AddRoomToCartCommand object</param>
        [HttpPost("add")]
        public async Task<IActionResult> AddRoomToCart([FromBody] AddRoomToCartCommand command)
        {
            if (command == null)
                return BadRequest("Invalid request payload.");

            var result = await _mediator.Send(command);

            if (!result.IsSuccess)
                return StatusCode(result.HttpStatusCode ?? 400, new
                {
                    success = false,
                    message = result.Error,
                    code = result.ErrorCode
                });

            return Ok(new
            {
                success = true,
                message = "Room added to cart successfully."
            });
        }
    }
}