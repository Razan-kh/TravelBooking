using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.Bookings.Commands;
using TravelBooking.Application.Cheackout.Commands;
using TravelBooking.Application.ViewingHotels.Queries;

namespace TravelBooking.Api.Hotels.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IMediator _mediator;
    public HotelsController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotelDetails(Guid id)
    {
        var res = await _mediator.Send(new GetHotelDetailsQuery(id));
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res.Error);
        return Ok(res.Value);
    }
}