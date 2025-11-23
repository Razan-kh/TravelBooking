using TravelBooking.Application.Common;
using TravelBooking.Application.Mappers;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Services.Interfaces;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Rooms.Interfaces;

namespace TravelBooking.Application.Rooms.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IRoomMapper _mapper;

    public RoomService(IRoomRepository roomRepo, IRoomMapper mapper)
    {
        _roomRepo = roomRepo;
        _mapper = mapper;
    }

    public async Task<List<RoomDto>> GetRoomsAsync(string? filter, int page, int pageSize, CancellationToken ct)
    {
        var allRooms = await _roomRepo.GetRoomsAsync(filter, ct);

        var pagedRooms = allRooms
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return pagedRooms.Select(_mapper.Map).ToList();
    }

    public async Task<RoomDto?> GetRoomByIdAsync(Guid id, CancellationToken ct)
    {
        var room = await _roomRepo.GetByIdAsync(id, ct);
        return room == null ? null : _mapper.Map(room);
    }

    public async Task<Guid> CreateRoomAsync(CreateRoomDto dto, CancellationToken ct)
    {
        var room = _mapper.Map(dto);
        room.Id = Guid.NewGuid();

        await _roomRepo.AddAsync(room, ct);

        return room.Id;
    }

    public async Task UpdateRoomAsync(UpdateRoomDto dto, CancellationToken ct)
    {
        var existing = await _roomRepo.GetByIdAsync(dto.Id, ct);
        if (existing == null)
            throw new KeyNotFoundException($"Room with ID {dto.Id} not found.");

        // Mapperly updates entity in place
        _mapper.UpdateRoomFromDto(dto, existing);

        await _roomRepo.UpdateAsync(existing, ct);
    }

    public async Task DeleteRoomAsync(Guid id, CancellationToken ct)
    {
        var existing = await _roomRepo.GetByIdAsync(id, ct);
        if (existing == null) return;

        await _roomRepo.DeleteAsync(existing, ct);
    }
}