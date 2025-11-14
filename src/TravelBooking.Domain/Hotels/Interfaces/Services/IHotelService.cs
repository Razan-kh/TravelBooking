using TravelBooking.Domain.Hotels.Entities;

public interface IHotelService
{
    Task<List<Hotel>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<Hotel> GetHotelByIdAsync(Guid id, CancellationToken ct);
    Task <Hotel> CreateHotelAsync(Hotel hotel, CancellationToken ct);
    Task UpdateHotelAsync(Hotel hotel, CancellationToken ct);
    Task DeleteHotelAsync(Guid id, CancellationToken ct);
}