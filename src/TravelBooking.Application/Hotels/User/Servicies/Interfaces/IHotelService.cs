using TravelBooking.Application.DTOs;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.ViewingHotels.Services.Interfaces;

public interface IHotelService
{
    Task<HotelDetailsDto?> GetHotelDetailsAsync(Guid hotelId, CancellationToken ct);
    Task<Result<List<FeaturedHotelDto>>> GetFeaturedDealsAsync(int count);
    Task<Result<List<RecentlyVisitedHotelDto>>> GetRecentlyVisitedHotelsAsync(Guid userId, int count);
}