using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.Cities.Commands;
using TravelBooking.Application.Cities.Dtos;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.Cities.Admin.Queries;
using TravelBooking.Api.Extensions;

namespace TravelBooking.Api.Cities.Admin.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/cities")]
public class CityController : ControllerBase
{
    private readonly IMediator _mediator;
    public CityController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedResult<CityDto>>> GetCities([FromQuery] string? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var res = await _mediator.Send(new GetCitiesQuery(filter, page, pageSize));
        return res.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CityDto>> GetCity(Guid id)
    {
        var res = await _mediator.Send(new GetCityByIdQuery(id));
        return res.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> CreateCity([FromBody] CreateCityDto dto)
    {
        var res = await _mediator.Send(new CreateCityCommand(dto));
        return res.ToActionResult(() => CreatedAtAction(nameof(GetCity), new { id = res.Value.Id }, res.Value));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateCity(Guid id, [FromBody] UpdateCityDto dto)
    {
        if (id != dto.Id) return BadRequest("Id mismatch");
        var res = await _mediator.Send(new UpdateCityCommand(dto));
        
        return res.IsSuccess ? NoContent() : res.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCity(Guid id)
    {
        var res = await _mediator.Send(new DeleteCityCommand(id));
        return res.IsSuccess ? NoContent() : res.ToActionResult();
    }
}