using TravelBooking.Application.Common;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Domain.Rooms.interfaces.Services;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.Rooms.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;

    public RoomService(IRoomRepository roomRepo)
    {
        _roomRepo = roomRepo;
    }

    public async Task<List<Room>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var allRooms = await _roomRepo.GetRoomsAsync(filter, ct);

        // Pagination
        var pagedRooms = allRooms
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return pagedRooms;
    }

    public async Task<Room?> GetRoomByIdAsync(Guid id, CancellationToken ct)
        => await _roomRepo.GetByIdAsync(id, ct);

    public async Task<Room> CreateRoomAsync(Room room, CancellationToken ct)
    {
        await _roomRepo.AddAsync(room, ct);
        return room;
    }

    public async Task UpdateRoomAsync(Room room, CancellationToken ct)
        => await _roomRepo.UpdateAsync(room, ct);

    public async Task DeleteRoomAsync(Guid id, CancellationToken ct)
    {
        var existing = await _roomRepo.GetByIdAsync(id, ct);
        if (existing == null) return;
        await _roomRepo.DeleteAsync(existing, ct);
    }
}