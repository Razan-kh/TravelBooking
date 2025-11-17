using MediatR;
using TravelBooking.Application.Common;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Queries;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.interfaces.Services;

namespace TravelBooking.Application.Rooms.Handlers;

public class GetRoomsHandler : IRequestHandler<GetRoomsQuery, Result<PagedResult<RoomDto>>>
{
    private readonly IRoomService _service;
    private readonly IRoomMapper _mapper;

    public GetRoomsHandler(IRoomService service, IRoomMapper mapper)
    {
        _service = service;
        _mapper = mapper;
    }

    public async Task<Result<PagedResult<RoomDto>>> Handle(GetRoomsQuery req, CancellationToken ct)
    {
        int page = req.Page <= 0 ? 1 : req.Page;
        int pageSize = req.PageSize <= 0 ? 20 : req.PageSize;

        // Call domain service
        var rooms = await _service.GetRoomsAsync(req.Filter, page, pageSize, ct);
System.Console.WriteLine(rooms[0].RoomCategory.ChildrenCapacity);

        // Map to DTOs
        var roomDtos = rooms.Select(r => _mapper.Map(r)).ToList();
System.Console.WriteLine(roomDtos[0].ChildrenCapacity);

        // Wrap in PagedResult
        var paged = new PagedResult<RoomDto>
        {
            Items = roomDtos,
            TotalCount = roomDtos.Count // or total count from repo if available
        };

        return Result<PagedResult<RoomDto>>.Success(paged);
    }
}