using Microsoft.AspNetCore.Mvc;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.TrendingCities.Queries;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Api.Extensions;

namespace TravelBooking.Api.Cities.User.Controllers;

[Authorize]
[ApiController]
[Route("api/cities")]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
    public CityController(IMediator mediator) => _mediator = mediator;

    [HttpGet("trending")]
    public async Task<ActionResult<List<TrendingCityDto>>> GetTrendingDestinations([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetTrendingCitiesQuery(count));
        return result.ToActionResult();
    }
}