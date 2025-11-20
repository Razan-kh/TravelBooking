using TravelBooking.Application.Queries;

namespace TravelBooking.Application.Searching.Interfaces;

public interface IHotelSearchService
{
    Task<PagedResult<HotelCardDto>> SearchAsync(SearchHotelsQuery query, CancellationToken ct);
}
