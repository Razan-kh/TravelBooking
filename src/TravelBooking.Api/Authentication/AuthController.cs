using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TravelBooking.Application.Users.Commands;
using TravelBooking.Application.Users.DTOs;

namespace TravelBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IMediator mediator) : ControllerBase
{
    private readonly IMediator _mediator = mediator;

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var cmd = new LoginCommand
            {
                UsernameOrEmail = request.UsernameOrEmail,
                Password = request.Password
            };

            var result = await _mediator.Send(cmd);
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            // Do NOT leak internal errors in production — this is for demo
            return Problem(detail: ex.Message);
        }
    }
}