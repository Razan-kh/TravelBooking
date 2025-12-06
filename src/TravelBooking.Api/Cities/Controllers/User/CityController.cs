using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.TrendingCities.Queries;

namespace TravelBooking.Api.Cities.User.Controllers;

[Authorize]
[ApiController]
[Route("api/city")]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
    public CityController(IMediator mediator) => _mediator = mediator;

    [HttpGet("trending-destinations")]
    public async Task<IActionResult> GetTrendingDestinations([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetTrendingCitiesQuery(count));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}