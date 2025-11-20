using MediatR;
using TravelBooking.Application.Queries;
using TravelBooking.Application.Searching.Servicies.Implementations;
using TravelBooking.Application.Searching.Servicies.Interfaces;

namespace TravelBooking.Application.Searching.Handlers;

public class SearchHotelsHandler 
    : IRequestHandler<SearchHotelsQuery, PagedResult<HotelCardDto>>
{
    private readonly IHotelService _service;

    public SearchHotelsHandler(IHotelService service)
    {
        _service = service;
    }

    public async Task<PagedResult<HotelCardDto>> Handle(
        SearchHotelsQuery request, CancellationToken cancellationToken)
    {
        return await _service.SearchAsync(request, cancellationToken);
    }
}
/*
public class SearchHotelsHandler : IRequestHandler<SearchHotelsQuery, PagedResult<HotelCardDto>>
{
    private readonly IAppDbContext _db;
    private readonly ISieveProcessor _sieveProcessor;

    public SearchHotelsHandler(IAppDbContext db, ISieveProcessor sieveProcessor)
    {
        _db = db;
        _sieveProcessor = sieveProcessor;
    }

    public async Task<PagedResult<HotelCardDto>> Handle(SearchHotelsQuery request, CancellationToken cancellationToken)
    {
        // Convert DateOnly to DateTime (UTC midnight) for comparison
        DateTime? checkIn = request.CheckIn?.ToDateTime(TimeOnly.MinValue);
        DateTime? checkOut = request.CheckOut?.ToDateTime(TimeOnly.MinValue);

        var totalGuests = Math.Max(1, request.Adults + request.Children);

        // Base query with includes needed for projection
        var baseQuery = _db.Hotels
            .AsNoTracking()
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Amenities)
            .Include(h => h.RoomCategories).ThenInclude(rc => rc.Rooms)
            .Include(h => h.City)
            .AsQueryable();

        // Basic filters
        if (!string.IsNullOrWhiteSpace(request.Keyword))
        {
            var kw = request.Keyword.Trim();
            baseQuery = baseQuery.Where(h => h.Name.Contains(kw) || h.City != null && h.City.Name.Contains(kw));
        }

        if (request.CityId.HasValue)
            baseQuery = baseQuery.Where(h => h.CityId == request.CityId.Value);

        if (request.MinStar.HasValue) baseQuery = baseQuery.Where(h => h.StarRating >= request.MinStar.Value);
        if (request.MaxStar.HasValue) baseQuery = baseQuery.Where(h => h.StarRating <= request.MaxStar.Value);

        // Price filter uses RoomCategory.PricePerNight
        if (request.MinPrice.HasValue || request.MaxPrice.HasValue)
        {
            var min = request.MinPrice;
            var max = request.MaxPrice;
            baseQuery = baseQuery.Where(h => h.RoomCategories.Any(rc =>
                (!min.HasValue || rc.PricePerNight >= min.Value) &&
                (!max.HasValue || rc.PricePerNight <= max.Value)));
        }

        // Amenities: require that at least one category provides all requested amenities
        if (request.Amenities != null && request.Amenities.Length > 0)
        {
            foreach (var amen in request.Amenities.Select(a => a.Trim()).Where(a => a != string.Empty))
            {
                var lower = amen.ToLower();
                baseQuery = baseQuery.Where(h => h.RoomCategories.Any(rc => rc.Amenities.Any(a => a.Name.ToLower() == lower)));
            }
        }

        // RoomType & capacity & availability — we need to ensure at least one RoomCategory satisfies criteria and has available rooms
if (request.RoomType.HasValue || totalGuests > 1 || (checkIn.HasValue && checkOut.HasValue))
{
    var checkInDate = request.CheckIn ?? default(DateOnly?);
    var checkOutDate = request.CheckOut ?? default(DateOnly?);

    baseQuery = baseQuery.Where(h => h.RoomCategories.Any(rc =>
        (!request.RoomType.HasValue || rc.RoomType == request.RoomType.Value)
        && (rc.AdultsCapacity + rc.ChildrenCapacity) >= totalGuests
        
        && (checkInDate == null || checkOutDate == null
            || !_db.Bookings.Any(b =>
                b.Rooms.Any(r => r.RoomCategoryId == rc.Id) &&
                b.CheckInDate < checkOutDate &&
                b.CheckOutDate > checkInDate))
                
    ));

}


        // Apply Sieve for filters & sorts but NOT pagination
        var filteredSorted = _sieveProcessor.Apply(request.SieveModel, baseQuery, applyPagination: false);

        // Cursor-based pagination
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        // Determine ordering
        // If the client provided sorts via SieveModel, try to respect them. For cursor stability, ensure Id is final tiebreaker.
        // We'll materialize ordering by applying the Sieve sort to an IQueryable and then use the primary sort field to build predicates.

        // For simplicity and reliability, we will use a deterministic ordering: if sorts provided, use them; otherwise default to StarRating desc, MinPrice asc, Id asc
        // But we need a projection with MinPrice to use in ordering — compute MinPrice subquery.

var withMinPrice = filteredSorted
    .Cast<Hotel>()   // 👈 force it to be Hotel, not TEntity
    .Select(h => new
    {
        Hotel = h,
        MinPrice = h.RoomCategories.Min(rc => (decimal?)rc.PricePerNight) ?? 0
    });


        // Determine the applied sorts string
        var sortsToken = request.SieveModel.Sorts; // can be null

        // Build ordered query (we'll fallback to default if no sorts)
        IOrderedQueryable<dynamic> ordered;

        if (!string.IsNullOrWhiteSpace(sortsToken))
        {
            // apply the sorts token by asking Sieve to sort; easiest way: apply Sieve with pagination disabled and no filters, but we already have applied Sieve
            // Unfortunately dynamic expression of sorts on anonymous types is complex; instead, for robust behavior we will:
            // - If sorts contain 'starRating' or 'price' we handle those cases explicitly.

            // Quick heuristic: if sorts contains "starRating" use that, if contains "minPrice" use MinPrice, else fallback to default.
            var s = sortsToken.ToLower();
            if (s.Contains("starrating") || s.Contains("starrating"))
            {
                // detect direction
                var desc = s.Contains("-starrating") || s.StartsWith("-");
                ordered = desc ? withMinPrice.OrderByDescending(x => x.Hotel.StarRating).ThenBy(x => x.MinPrice).ThenBy(x => x.Hotel.Id)
                               : withMinPrice.OrderBy(x => x.Hotel.StarRating).ThenBy(x => x.MinPrice).ThenBy(x => x.Hotel.Id);
            }
            else if (s.Contains("price") || s.Contains("minprice") || s.Contains("pricepernight"))
            {
                var desc = s.Contains("-price") || s.Contains("-minprice");
                ordered = desc ? withMinPrice.OrderByDescending(x => x.MinPrice).ThenByDescending(x => x.Hotel.StarRating).ThenBy(x => x.Hotel.Id)
                               : withMinPrice.OrderBy(x => x.MinPrice).ThenByDescending(x => x.Hotel.StarRating).ThenBy(x => x.Hotel.Id);
            }
            else
            {
                // fallback
                ordered = withMinPrice.OrderByDescending(x => x.Hotel.StarRating).ThenBy(x => x.MinPrice).ThenBy(x => x.Hotel.Id);
            }
        }
        else
        {
            ordered = withMinPrice.OrderByDescending(x => x.Hotel.StarRating).ThenBy(x => x.MinPrice).ThenBy(x => x.Hotel.Id);
        }

        // Apply cursor predicate if present
        CursorPayload? last = CursorHelper.Decode<CursorPayload>(request.Cursor);
        if (last != null)
        {
            // Build predicate using last values (starRating, minPrice, id)
            ordered = ApplyCursorPredicate(ordered, last);
        }

        // Grab pageSize + 1 to detect if there is a next page
        var fetched = await ordered.Take(pageSize + 1).ToListAsync(cancellationToken);

        string? nextCursor = null;
        var pageItems = fetched.Take(pageSize).ToList();
        if (fetched.Count > pageSize)
        {
            // Build next cursor from the last returned item
            var lastItem = fetched[pageSize - 1];
            var payload = new CursorPayload
            {
                StarRating = (int)lastItem.Hotel.StarRating,
                MinPrice = (decimal)(lastItem.MinPrice ?? 0),
                LastId = (Guid)lastItem.Hotel.Id
            };
            nextCursor = CursorHelper.Encode(payload);
        }

        // Project pageItems to DTOs
        // Project pageItems to DTOs
      var data = pageItems.Select(x => new HotelCardDto
{
    Id = x.Hotel.Id,
    Name = x.Hotel.Name,
    ThumbnailUrl = x.Hotel.ThumbnailUrl,
    City = x.Hotel.City != null ? x.Hotel.City.Name : string.Empty,
    StarRating = x.Hotel.StarRating,
    MinPrice = x.MinPrice,
    Amenities = (x.Hotel.RoomCategories as IEnumerable<RoomCategory> ?? Enumerable.Empty<RoomCategory>())
        .SelectMany(rc => rc.Amenities)
        .Select(a => a.Name)
        .Distinct()
        .ToList()
}).ToList();


            var meta = new { PageSize = pageSize, NextCursor = nextCursor };

        return new PagedResult<HotelCardDto>
        {
            Meta = meta,
            Data = data
        };
    }

    // Builds an EF-friendly predicate stub for availability — expressed through SQL subqueries
    // NOTE: Use raw LINQ subqueries to express 'room count - bookedRooms > 0'
    private static bool RoomCategoryHasAvailabilityPredicate(Guid hotelId, Guid roomCategoryId, DateTime? checkInUtc, DateTime? checkOutUtc)
    {
        // This method will never be executed locally because we use its expression form in LINQ Where above.
        // It's here only for readability; the actual predicate is constructed in the extension method below.
        return true;
    }

    // Apply cursor predicate to ordered query
    private static IOrderedQueryable<dynamic> ApplyCursorPredicate(IOrderedQueryable<dynamic> ordered, CursorPayload last)
    {
        // ordered is an IQueryable of anonymous { Hotel, MinPrice }
        // We'll reapply where clause using expression trees would be ideal; for simplicity we'll convert to IQueryable and filter using LINQ methods.

        // Build predicate: (Hotel.StarRating < last.StarRating)
        // OR (Hotel.StarRating == last.StarRating AND MinPrice > last.MinPrice)
        // OR (Hotel.StarRating == last.StarRating AND MinPrice == last.MinPrice AND Hotel.Id > last.LastId)

        IQueryable<HotelCardDto> queryable = (IQueryable<HotelCardDto>)ordered;

        queryable = queryable.Where(x =>
            x.StarRating < last.StarRating ||
            (x.StarRating == last.StarRating && x.MinPrice > last.MinPrice) ||
            (x.StarRating == last.StarRating && x.MinPrice == last.MinPrice && x.Id > last.LastId)
        );



        // Need to preserve ordering — reapply the ordering
        var reordered = queryable.OrderByDescending(x => x.StarRating).ThenBy(x => x.MinPrice).ThenBy(x => x.Id) as IOrderedQueryable<dynamic>;
        return reordered;
    }
}
*/