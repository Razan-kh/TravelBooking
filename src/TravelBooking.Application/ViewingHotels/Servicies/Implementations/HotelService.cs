using TravelBooking.Application.DTOs;
using TravelBooking.Application.ViewingHotels.Mappers;
using TravelBooking.Application.ViewingHotels.Services.Interfaces;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;

namespace TravelBooking.Application.ViewingHotels.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _repo;
    private readonly IHotelMapper _hotelMapper;

    public HotelService(IHotelRepository repo, IHotelMapper hotelMapper)
    {
        _repo = repo;
        _hotelMapper = hotelMapper;
    }

    public async Task<HotelDetailsDto?> GetHotelDetailsAsync(Guid hotelId, CancellationToken ct)
    {
        var hotel = await _repo.GetByIdAsync(hotelId, ct);
        if (hotel is null) return null;

        var dto = _hotelMapper.Map(hotel);

        return dto;
    }
}