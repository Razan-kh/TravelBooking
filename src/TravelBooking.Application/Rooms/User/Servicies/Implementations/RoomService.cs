using TravelBooking.Application.DTOs;
using TravelBooking.Application.Rooms.User.Mappers.Interfaces;
using TravelBooking.Application.Rooms.User.Servicies.Interfaces;
using TravelBooking.Domain.Rooms.Interfaces;

namespace TravelBooking.Application.Rooms.User.Servicies.Implementations;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _repo;
    private readonly IRoomCategoryMapper _mapper;

    public RoomService(IRoomRepository repo, IRoomCategoryMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<RoomCategoryDto>> GetRoomCategoriesWithAvailabilityAsync(
        Guid hotelId,
        DateOnly? checkIn,
        DateOnly? checkOut,
        CancellationToken ct)
    {
        var roomCategories = await _repo.GetRoomCategoriesByHotelIdAsync(hotelId, ct);
        var result = new List<RoomCategoryDto>();

        foreach (var rc in roomCategories)
        {
            var dto = _mapper.Map(rc);

            // CASE 1: No dates → return all rooms
            if (checkIn is null || checkOut is null)
            {
                dto.AvailableRooms = await _repo.CountTotalRoomsAsync(rc.Id, ct);
            }
            else
            {
                dto.AvailableRooms = await _repo.CountAvailableRoomsAsync(
                    rc.Id,
                    checkIn.Value,
                    checkOut.Value,
                    ct);
            }

            result.Add(dto);
        }

        return result;
    }
}