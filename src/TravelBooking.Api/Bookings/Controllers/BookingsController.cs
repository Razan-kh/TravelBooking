/*
[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IMediator _mediator;
    public BookingsController(IMediator mediator) => _mediator = mediator;

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateBookingCommand cmd)
    {
        var res = await _mediator.Send(cmd);
        if (!res.IsSuccess) return StatusCode(res.HttpStatusCode ?? 400, res.Error);
        return Ok(res.Value);
    }
}
*/