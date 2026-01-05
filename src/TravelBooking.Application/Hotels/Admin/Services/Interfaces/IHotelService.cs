using TravelBooking.Application.Hotels.Dtos;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Admin.Servicies.Interfaces;
 
public interface IHotelService
{
    Task<List<HotelDto>> GetHotelsAsync(string? filter, int page, int pageSize, CancellationToken ct);
    Task<HotelDto?> GetHotelByIdAsync(Guid id, CancellationToken ct);
    Task<HotelDto> CreateHotelAsync(CreateHotelDto dto, CancellationToken ct);
    Task<Result> UpdateHotelAsync(UpdateHotelDto dto, CancellationToken ct);
    Task<Result> DeleteHotelAsync(Guid id, CancellationToken ct);
}