using TravelBooking.Application.Hotels.Dtos;

namespace TravelBooking.Application.Hotels.Servicies;
 
public interface IHotelService
{
    Task<List<HotelDto>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<HotelDto?> GetHotelByIdAsync(Guid id, CancellationToken ct);
    Task<HotelDto> CreateHotelAsync(CreateHotelDto dto, CancellationToken ct);
    Task UpdateHotelAsync(UpdateHotelDto dto, CancellationToken ct);
    Task DeleteHotelAsync(Guid id, CancellationToken ct);
}