using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.ViewingHotels.Queries;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Api.Extensions;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.FeaturedDeals.Dtos;

namespace TravelBooking.Api.Hotels.User.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IMediator _mediator;
    public HotelController(IMediator mediator) => _mediator = mediator;

    [HttpGet("details/{id}")]
    public async Task<ActionResult<HotelDetailsDto>> GetHotelDetails(Guid id, DateOnly? checkIn, DateOnly? checkOut)
    {
        var res = await _mediator.Send(new GetHotelDetailsQuery(id, checkIn, checkOut));
        return res.ToActionResult();
    }

    [HttpGet("recently-visited/{userId}")]
    public async Task<ActionResult<List<RecentlyVisitedHotelDto>>> GetRecentlyVisitedHotels(Guid userId, [FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetRecentlyVisitedHotelsQuery(userId, count));
        return result.ToActionResult();
    }

    [HttpGet("featured-deals")]
    public async Task<ActionResult<List<FeaturedHotelDto>>> GetFeaturedDeals([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetFeaturedDealsQuery(count));
        return result.ToActionResult();
    }
}