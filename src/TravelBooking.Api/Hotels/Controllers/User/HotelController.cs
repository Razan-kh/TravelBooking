using Microsoft.AspNetCore.Mvc;
using MediatR;
using TravelBooking.Application.ViewingHotels.Queries;
using Microsoft.AspNetCore.Authorization;
using TravelBooking.Application.FeaturedDeals.Queries;
using TravelBooking.Application.RecentlyVisited.Queries;
using TravelBooking.Api.Extensions;
using TravelBooking.Application.DTOs;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.FeaturedDeals.Dtos;
using Sieve.Models;
using TravelBooking.Domain.Rooms.Enums;

namespace TravelBooking.Api.Hotels.User.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly IMediator _mediator;
    public HotelController(IMediator mediator) => _mediator = mediator;

    [HttpGet("details/{id}")]
    public async Task<ActionResult<HotelDetailsDto>> GetHotelDetails(Guid id, DateOnly? checkIn, DateOnly? checkOut)
    {
        var res = await _mediator.Send(new GetHotelDetailsQuery(id, checkIn, checkOut));
        return res.ToActionResult();
    }

    [HttpGet("recently-visited/{userId}")]
    public async Task<ActionResult<List<RecentlyVisitedHotelDto>>> GetRecentlyVisitedHotels(Guid userId, [FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetRecentlyVisitedHotelsQuery(userId, count));
        return result.ToActionResult();
    }

    [HttpGet("featured-deals")]
    public async Task<ActionResult<List<FeaturedHotelDto>>> GetFeaturedDeals([FromQuery] int count = 5)
    {
        var result = await _mediator.Send(new GetFeaturedDealsQuery(count));
        return result.ToActionResult();
    }
    
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
        [FromQuery] string? amenities, 
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
}