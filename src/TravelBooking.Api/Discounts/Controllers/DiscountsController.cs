using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Api.Extensions;
using TravelBooking.Application.Discounts.Commands;
using TravelBooking.Application.Discounts.Dtos;

namespace TravelBooking.Api.Discounts.Controllers;

[ApiController]
[Route("api/hotels/{hotelId:guid}/room-categories/{roomCategoryId:guid}/discounts")]
public class DiscountsController : ControllerBase
{
    private readonly IMediator _mediator;
    public DiscountsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountDto>>> GetAll([FromRoute] Guid hotelId, [FromRoute] Guid roomCategoryId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDiscountsQuery(hotelId, roomCategoryId), ct);
        return result.ToActionResult();
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DiscountDto>> GetById([FromRoute] Guid hotelId, [FromRoute] Guid roomCategoryId, [FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDiscountByIdQuery(hotelId, roomCategoryId, id), ct);
        return result.ToActionResult();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromRoute] Guid hotelId, [FromRoute] Guid roomCategoryId, [FromBody] CreateDiscountDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _mediator.Send(new CreateDiscountCommand(hotelId, roomCategoryId, dto), ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { hotelId, roomCategoryId, id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid hotelId, [FromRoute] Guid roomCategoryId, [FromRoute] Guid id, [FromBody] UpdateDiscountDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != dto.Id) return BadRequest("Id in route and body must match.");
        var result = await _mediator.Send(new UpdateDiscountCommand(hotelId, roomCategoryId, dto), ct);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid hotelId, [FromRoute] Guid roomCategoryId, [FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteDiscountCommand(hotelId, roomCategoryId, id), ct);
        if (!result.IsSuccess) return NotFound(result.Error);
        return NoContent();
    }
}