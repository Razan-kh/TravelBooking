using MediatR;
using Sieve.Models;
using System;

namespace TravelBooking.Application.Queries;

public class SearchHotelsQuery : IRequest<PagedResult<TravelBooking.Application.DTOs.HotelCardDto>>
{
    public SieveModel SieveModel { get; set; } = new SieveModel();
    public string? Keyword { get; set; }
    public Guid? CityId { get; set; }
    public int? MinStar { get; set; }
    public int? MaxStar { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public string[]? Amenities { get; set; }
    public RoomType? RoomType { get; set; }
    public int Adults { get; set; } = 2;
    public int Children { get; set; } = 0;
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }

    // Cursor paging
    public string? Cursor { get; set; }
    public int PageSize { get; set; } = 20; // default page size
}