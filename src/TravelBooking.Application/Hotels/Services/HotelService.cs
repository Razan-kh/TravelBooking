using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Repositories;

namespace TravelBooking.Application.Hotels.Servicies;

public class HotelService : IHotelService
{
    private readonly IHotelRepository _hotelRepo;

    public HotelService(IHotelRepository hotelRepo)
    {
        _hotelRepo = hotelRepo;
    }

    public async Task<List<Hotel>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct)
        => await _hotelRepo.GetHotelsAsync(filter, page, pageSize, ct);

    public async Task<Hotel?> GetHotelByIdAsync(Guid id, CancellationToken ct)
        => await _hotelRepo.GetByIdAsync(id, ct);

    public async Task<Hotel> CreateHotelAsync(Hotel hotel, CancellationToken ct)
    {
        hotel.Id = Guid.NewGuid();
        await _hotelRepo.AddAsync(hotel, ct);
        return hotel;
    }

    public async Task UpdateHotelAsync(Hotel hotel, CancellationToken ct)
        => await _hotelRepo.UpdateAsync(hotel, ct);

    public async Task DeleteHotelAsync(Guid id, CancellationToken ct)
    {
        var existing = await _hotelRepo.GetByIdAsync(id, ct);
        if (existing == null) return;
        await _hotelRepo.DeleteAsync(existing, ct);
    }
}