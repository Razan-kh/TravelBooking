using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.TrendingCities.Dtos;
using TravelBooking.Application.TrendingCities.Mappers;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;

namespace TravelBooking.Application.Hotels.Servicies;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepo;
    private readonly IHotelMapper _mapper;

    public HotelService(IHotelRepository hotelRepo, IHotelMapper mapper)
    {
        _hotelRepo = hotelRepo;
        _mapper = mapper;
    }

    public async Task<List<HotelDto>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var hotels = await _hotelRepo.GetHotelsAsync(filter, page, pageSize, ct);
        return hotels.Select(h => _mapper.Map(h)).ToList();
    }

    public async Task<HotelDto?> GetHotelByIdAsync(Guid id, CancellationToken ct)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id, ct);
        return hotel == null ? null : _mapper.Map(hotel);
    }

    public async Task<HotelDto> CreateHotelAsync(CreateHotelDto dto, CancellationToken ct)
    {
        var hotel = _mapper.Map(dto);
        hotel.Id = Guid.NewGuid();
        await _hotelRepo.AddAsync(hotel, ct);
        return _mapper.Map(hotel);
    }

    public async Task UpdateHotelAsync(UpdateHotelDto dto, CancellationToken ct)
    {
        var hotel = await _hotelRepo.GetByIdAsync(dto.Id, ct);
        if (hotel == null)
            throw new KeyNotFoundException($"Hotel with ID {dto.Id} not found.");

        _mapper.UpdateHotelFromDto(dto, hotel);
        await _hotelRepo.UpdateAsync(hotel, ct);
    }

    public async Task DeleteHotelAsync(Guid id, CancellationToken ct)
    {
        var hotel = await _hotelRepo.GetByIdAsync(id, ct);
        if (hotel == null) return;
        await _hotelRepo.DeleteAsync(hotel, ct);
    }


}