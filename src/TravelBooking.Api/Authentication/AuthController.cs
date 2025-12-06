using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.DTOs;
using TravelBooking.Api.Extensions;

namespace TravelBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
            var cmd = new LoginCommand
            {
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(cmd);
            return result.ToActionResult();
    }
}