using TravelBooking.Application.DTOs;

namespace TravelBooking.Application.ViewingHotels.Services.Interfaces;

public interface IHotelService
{
    Task<HotelDetailsDto?> GetHotelDetailsAsync(Guid hotelId, CancellationToken ct);
}