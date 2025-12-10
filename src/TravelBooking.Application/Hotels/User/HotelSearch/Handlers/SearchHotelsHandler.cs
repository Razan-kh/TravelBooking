using MediatR;
using TravelBooking.Application.Hotels.Admin.DTOs;
using TravelBooking.Application.Queries;
using TravelBooking.Application.Searching.Servicies.Interfaces;

namespace TravelBooking.Application.Searching.Handlers;

public class SearchHotelsHandler 
    : IRequestHandler<SearchHotelsQuery, PagedResult<HotelCardDto>>
{
    private readonly IHotelSearchService _service;

    public SearchHotelsHandler(IHotelSearchService service)
    {
        _service = service;
    }

    public async Task<PagedResult<HotelCardDto>> Handle(
        SearchHotelsQuery request, CancellationToken cancellationToken)
    {
        return await _service.SearchAsync(request, cancellationToken);
    }
}