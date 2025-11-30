using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Application.TrendingCities.Queries;
using Microsoft.AspNetCore.Authorization;

namespace TravelBooking.Api.Cities.Admin.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/city")]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
    public CityController(IMediator mediator) => _mediator = mediator;

    [HttpGet("cities")]
    public async Task<IActionResult> GetCities([FromQuery] string? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var res = await _mediator.Send(new GetCitiesQuery(filter, page, pageSize));
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res);
        return Ok(res.Value);
    }

    [HttpGet("cities/{id:guid}")]
    public async Task<IActionResult> GetCity(Guid id)
    {
        var res = await _mediator.Send(new GetCityByIdQuery(id));
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res);
        return Ok(res.Value);
    }

    [HttpPost("cities")]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityDto dto)
    {
        var res = await _mediator.Send(new CreateCityCommand(dto));
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res);
        return CreatedAtAction(nameof(GetCity), new { id = res.Value.Id }, res.Value);
    }

    [HttpPut("cities/{id:guid}")]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch");
        var res = await _mediator.Send(new UpdateCityCommand(dto));
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res);
        return NoContent();
    }

    [HttpDelete("cities/{id:guid}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        var res = await _mediator.Send(new DeleteCityCommand(id));
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res);
        return NoContent();
    }
}