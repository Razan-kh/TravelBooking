using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Api.Extensions;

namespace TravelBooking.Api.Hotels.Admin.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IMediator _mediator;

    public HotelController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateHotel([FromBody] CreateHotelCommand command)
    {
        var result = await _mediator.Send(command);
        
        return result.ToActionResult(() => 
        CreatedAtAction(nameof(GetHotelById), new { id = result.Value.Id }, result.Value));
    }

    [HttpGet]
    public async Task<ActionResult<List<HotelDto>>> GetHotels([FromQuery] string? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetHotelsQuery(filter, page, pageSize));
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<HotelDto>> GetHotelById(Guid id)
    {
        var result = await _mediator.Send(new GetHotelByIdQuery(id));
        return result.ToActionResult();
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] UpdateHotelDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(new UpdateHotelCommand(dto));
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        var result = await _mediator.Send(new DeleteHotelCommand(id));
        return result.IsSuccess ? NoContent() : result.ToActionResult();
    }
}