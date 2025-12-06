using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.ViewingHotels.Queries;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.RecentlyVisited.Queries;

namespace TravelBooking.Api.Hotels.User.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IMediator _mediator;
    public HotelController(IMediator mediator) => _mediator = mediator;

    [HttpGet("{id}")]
    public async Task<IActionResult> GetHotelDetails(Guid id, DateOnly? checkIn, DateOnly? checkOut)
    {
        var res = await _mediator.Send(new GetHotelDetailsQuery(id, checkIn, checkOut));
        return !res.IsSuccess ? StatusCode(res.HttpStatusCode ?? 400, res.Error) : (IActionResult)Ok(res.Value);
    }

    [HttpGet("recently-visited/{userId}")]
    public async Task<IActionResult> GetRecentlyVisitedHotels(Guid userId, [FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetRecentlyVisitedHotelsQuery(userId, count));
        return Ok(result.Value);
    }

    [HttpGet("featured-deals")]
    public async Task<IActionResult> GetFeaturedDeals([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetFeaturedDealsQuery(count));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}