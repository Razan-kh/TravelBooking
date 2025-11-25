using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Hotels.Commands;
using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Hotels.Queries;
using Microsoft.AspNetCore.Authorization;

namespace TravelBooking.Api.Hotels.Controllers;

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
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetHotelById), new { id = result.Value.Id }, result.Value) 
            : BadRequest(result.Error);
    }

    [HttpGet]
    public async Task<IActionResult> GetHotels([FromQuery] string? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetHotelsQuery(filter, page, pageSize));
        return Ok(result.Value);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHotelById(Guid id)
    {
        var result = await _mediator.Send(new GetHotelByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] UpdateHotelDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        var result = await _mediator.Send(new UpdateHotelCommand(dto));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        var result = await _mediator.Send(new DeleteHotelCommand(id));
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}