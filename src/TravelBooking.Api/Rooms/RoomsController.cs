using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Rooms.Commands;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.Images.Dtos;

namespace TravelBooking.Api.Rooms.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IMediator _mediator;

    public RoomsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetRooms([FromQuery] string? filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await _mediator.Send(new GetRoomsQuery(filter, page, pageSize));
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        var result = await _mediator.Send(new GetRoomByIdQuery(id));
        return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomCommand command)
    {
        var result = await _mediator.Send(command);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var result = await _mediator.Send(command);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        var result = await _mediator.Send(new DeleteRoomCommand(id));
        return result.IsSuccess ? NoContent() : NotFound(result.Error);
    }

    [HttpPost]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10MB
    [Route("api/rooms/{roomId:guid}/images")]
    public async Task<IActionResult> UploadRoomImage(
        Guid roomId, 
        [FromForm] ImageUploadDto imageUploadDto)
    {
        if (imageUploadDto.File == null || imageUploadDto.File.Length == 0)
            return BadRequest("File is required");

        var command = new UploadRoomImageCommand(roomId, imageUploadDto);
        var result = await _mediator.Send(command);
        
        return result.IsSuccess 
            ? Ok(result.Value) 
            : BadRequest(result.Error);
    }
}