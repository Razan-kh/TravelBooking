using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.Cities.Admin.Queries;

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
        return !res.IsSuccess ? StatusCode(res.HttpStatusCode ?? 400, res) : Ok(res.Value);
    }

    [HttpGet("cities/{id:guid}")]
    public async Task<IActionResult> GetCity(Guid id)
    {
        var res = await _mediator.Send(new GetCityByIdQuery(id));
        return !res.IsSuccess ? StatusCode(res.HttpStatusCode ?? 400, res) : (IActionResult)Ok(res.Value);
    }

    [HttpPost("cities")]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityDto dto)
    {
        var res = await _mediator.Send(new CreateCityCommand(dto));
        return !res.IsSuccess
            ? StatusCode(res.HttpStatusCode ?? 400, res)
            : (IActionResult)CreatedAtAction(nameof(GetCity), new { id = res.Value.Id }, res.Value);
    }

    [HttpPut("cities/{id:guid}")]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch");
        var res = await _mediator.Send(new UpdateCityCommand(dto));
        return !res.IsSuccess ? StatusCode(res.HttpStatusCode ?? 400, res) : NoContent();
    }

    [HttpDelete("cities/{id:guid}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        var res = await _mediator.Send(new DeleteCityCommand(id));
        return !res.IsSuccess ? StatusCode(res.HttpStatusCode ?? 400, res) : NoContent();
    }
}