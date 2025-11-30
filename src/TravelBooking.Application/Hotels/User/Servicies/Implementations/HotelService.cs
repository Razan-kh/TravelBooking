using TravelBooking.Application.DTOs;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Application.FeaturedDeals.Mappers;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Application.RecentlyVisited.Mappers;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Hotels.User.ViewingHotels.Mappers.Interfaces;

namespace TravelBooking.Application.ViewingHotels.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repo;
    private readonly IHotelMapper _hotelMapper;
    private readonly IFeaturedHotelMapper _featuredHotelMapper;
    private readonly IRecentlyVisitedHotelMapper _recentlyVisitedHotelMapper;
    public HotelService(IHotelRepository repo, IHotelMapper hotelMapper, IFeaturedHotelMapper featuredHotelMapper, IRecentlyVisitedHotelMapper recentlyVisitedHotelMapper)
    {
        _repo = repo;
        _hotelMapper = hotelMapper;
        _featuredHotelMapper = featuredHotelMapper;
        _recentlyVisitedHotelMapper = recentlyVisitedHotelMapper;
    }

    public async Task<HotelDetailsDto?> GetHotelDetailsAsync(Guid hotelId, CancellationToken ct)
    {
        var hotel = await _repo.GetByIdAsync(hotelId, ct);
        if (hotel is null) return null;

        var dto = _hotelMapper.Map(hotel);

        return dto;
    }

    public async Task<Result<List<FeaturedHotelDto>>> GetFeaturedDealsAsync(int count)
    {
        var hotels = await _repo.GetFeaturedHotelsAsync(count);
        var result = hotels.Select(_featuredHotelMapper.ToFeaturedHotelDto).ToList();
        return Result.Success(result);
    }

    public async Task<Result<List<RecentlyVisitedHotelDto>>> GetRecentlyVisitedHotelsAsync(Guid userId, int count)
    {
        var hotels = await _repo.GetRecentlyVisitedHotelsAsync(userId, count);
        var result = hotels.Select(_recentlyVisitedHotelMapper.ToRecentlyVisitedHotelDto).ToList();
        return Result.Success(result);
    }
}