using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.Bookings.Commands;
using TravelBooking.Application.Cheackout.Commands;
using Microsoft.AspNetCore.Authorization;

namespace TravelBooking.Api.Cheackout.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly IMediator _mediator;

    public CheckoutController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Confirm booking, process payment, send email and invoice.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);

        if (!result.IsSuccess)
            return StatusCode(result.HttpStatusCode ?? 400, result);

        return Ok(result);
    }
}