using Sieve.Services;
using TravelBooking.Application.Hotels.Admin.DTOs;
using TravelBooking.Application.Queries;
using TravelBooking.Application.Searching.Servicies.Interfaces;
using TravelBooking.Application.Utils;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Searching.Entities;

namespace TravelBooking.Application.Searching.Servicies.Implementations;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repo;
    private readonly ISieveProcessor _sieve;

    public HotelService(IHotelRepository repo, ISieveProcessor sieve)
    {
        _repo = repo;
        _sieve = sieve;
    }

    public async Task<PagedResult<HotelCardDto>> SearchAsync(SearchHotelsQuery request, CancellationToken ct)
    {
        var query = _repo.Query();

        // ---- all your filters here ----
        query = ApplyBasicFilters(query, request);
        query = ApplyAmenities(query, request);
        query = ApplyPriceFilter(query, request);
        query = ApplyRoomAvailability(query, request);

        var filteredSorted = (request.SieveModel is not null)
            ? _sieve.Apply(request.SieveModel, query, null)
            : query;

        // cursor paging
        var results = await ApplyCursorPagingAsync(filteredSorted, request, ct);

        return results;
    }

    private IQueryable<Hotel> ApplyBasicFilters(IQueryable<Hotel> query, SearchHotelsQuery request)
    {
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var kw = request.Keyword.Trim();
            query = query.Where(h =>
                h.Name.ToLower().Contains(kw.ToLower()) ||
                h.City!.Name.ToLower().Contains(kw.ToLower())); 
        }

        if (request.CityId.HasValue)
            query = query.Where(h => h.CityId == request.CityId);

        if (request.MinStar.HasValue)
            query = query.Where(h => h.StarRating >= request.MinStar);

        if (request.MaxStar.HasValue)
            query = query.Where(h => h.StarRating <= request.MaxStar);

        return query;
    }

    private IQueryable<Hotel> ApplyPriceFilter(IQueryable<Hotel> query, SearchHotelsQuery request)
    {
        if (!request.MinPrice.HasValue && !request.MaxPrice.HasValue)
            return query;

        var min = request.MinPrice;
        var max = request.MaxPrice;

        return query.Where(h => h.RoomCategories.Any(rc =>
              (!min.HasValue || rc.PricePerNight >= min)
           && (!max.HasValue || rc.PricePerNight <= max)));
    }

    private IQueryable<Hotel> ApplyAmenities(IQueryable<Hotel> query, SearchHotelsQuery request)
    {
        if (request.Amenities is null || request.Amenities.Length == 0)
            return query;

        foreach (string amen in request.Amenities)
        {
            var a = amen.ToLower().Trim();
            query = query.Where(h =>
                h.RoomCategories.Any(rc =>
                    rc.Amenities.Any(am => am.Name.ToLower() == a)));
        }

        return query;
    }

    private IQueryable<Hotel> ApplyRoomAvailability(IQueryable<Hotel> query, SearchHotelsQuery request)
    {
        if (!request.CheckIn.HasValue || !request.CheckOut.HasValue)
            return query;

        int guests = Math.Max(1, request.Adults + request.Children);
        var checkIn = request.CheckIn.Value;
        var checkOut = request.CheckOut.Value;

        return query.Where(h =>
            h.RoomCategories.Any(rc =>
                rc.AdultsCapacity + rc.ChildrenCapacity >= guests
            )
        );
    }

    private async Task<PagedResult<HotelCardDto>> ApplyCursorPagingAsync(
        IQueryable<Hotel> query, SearchHotelsQuery request, CancellationToken ct)
    {
        int pageSize = Math.Clamp(request.PageSize, 1, 100);

        var items = await _repo.ExecutePagedQueryAsync(query, pageSize + 1, ct);

        // create paged result and next cursor
        var dto = items.Take(pageSize).Select(h => new HotelCardDto
        {
            Id = h.Id,
            Name = h.Name,
            ThumbnailUrl = h.ThumbnailUrl,
            City = h.City?.Name ?? "",
            StarRating = h.StarRating,
            MinPrice = h.RoomCategories.Min(rc => rc.PricePerNight),
            Amenities = h.RoomCategories
                .SelectMany(rc => rc.Amenities)
                .Select(a => a.Name)
                .Distinct()
                .ToList()
        }).ToList();

        return new PagedResult<HotelCardDto>
        {
            Data = dto,
            Meta = new
            {
                PageSize = pageSize,
                NextCursor = items.Count > pageSize
                    ? CursorHelper.Encode(new CursorPayload
                    {
                        LastId = items[pageSize - 1].Id
                    })
                    : null
            }
        };
    }
}