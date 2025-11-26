using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Application.TrendingCities.Queries;
using Microsoft.AspNetCore.Authorization;

namespace TravelBooking.Api.HomePage.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HomeController : ControllerBase
{
    private readonly IMediator _mediator;

    public HomeController(IMediator mediator) => _mediator = mediator;

    [HttpGet("featured-deals")]
    public async Task<IActionResult> GetFeaturedDeals([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetFeaturedDealsQuery(count));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("recently-visited/{userId}")]
    public async Task<IActionResult> GetRecentlyVisitedHotels(Guid userId, [FromQuery] int count = 5)
    {
       // var result = await _mediator.Send(new GetRecentlyVisitedHotelsQuery(userId, count));
        
       // return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
           try
    {
        var result = await _mediator.Send(new GetRecentlyVisitedHotelsQuery(userId, count));
        return Ok(result.Value);
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.ToString());
    }
    }

    [HttpGet("trending-destinations")]
    public async Task<IActionResult> GetTrendingDestinations([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetTrendingCitiesQuery(count));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}