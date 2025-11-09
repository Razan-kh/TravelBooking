using Microsoft.AspNetCore.Mvc;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Sieve.Models;
using System;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Enums;

namespace TravelBooking.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HotelsController : ControllerBase
{
    private readonly IMediator _mediator;
    public HotelsController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Search hotels with filters and cursor-based infinite scroll.
    /// </summary>

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] SieveModel sieveModel,
        [FromQuery] string? keyword,
        [FromQuery] Guid? cityId,
        [FromQuery] int? minStar,
        [FromQuery] int? maxStar,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? amenities, // comma separated
        [FromQuery] RoomType? roomType,
        [FromQuery] int adults = 2,
        [FromQuery] int children = 0,
        [FromQuery] DateOnly? checkIn = null,
        [FromQuery] DateOnly? checkOut = null,
        [FromQuery] string? cursor = null,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var query = new TravelBooking.Application.Queries.SearchHotelsQuery
        {
            SieveModel = sieveModel,
            Keyword = keyword,
            CityId = cityId,
            MinStar = minStar,
            MaxStar = maxStar,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            Amenities = string.IsNullOrWhiteSpace(amenities) ? null : amenities.Split(',', StringSplitOptions.RemoveEmptyEntries),
            RoomType = roomType,
            Adults = adults,
            Children = children,
            CheckIn = checkIn,
            CheckOut = checkOut,
            Cursor = cursor,
            PageSize = pageSize
        };

        var res = await _mediator.Send(query, cancellationToken);
        return Ok(res);
    }

    [HttpGet("hi")]
    public async Task<IActionResult> Hi()
    {
        return Ok();
    }

    [HttpGet("test")]
public IActionResult Test() => Ok("Routing works!");

}

