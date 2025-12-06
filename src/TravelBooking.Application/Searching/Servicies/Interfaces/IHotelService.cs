using TravelBooking.Application.Hotels.Admin.DTOs;
using TravelBooking.Application.Queries;

namespace TravelBooking.Application.Searching.Servicies.Interfaces;

public interface IHotelService
{
    Task<PagedResult<HotelCardDto>> SearchAsync(SearchHotelsQuery query, CancellationToken ct);
}
