using TravelBooking.Application.Mappers;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Application.Rooms.Services.Interfaces;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Domain.Rooms.Interfaces;
using Microsoft.AspNetCore.Http;
using TravelBooking.Domain.Images.Entities;
using Microsoft.Extensions.FileProviders;
using TravelBooking.Domain.Images.Dtos;
using TravelBooking.Application.Images.Servicies;
using Microsoft.Extensions.Logging;
using TravelBooking.Domain.Shared.Interfaces;
using TravelBooking.Domain.Images.interfaces;

namespace TravelBooking.Application.Rooms.Services;

public class RoomService : IRoomService
{
    private readonly IRoomRepository _roomRepo;
    private readonly IRoomMapper _mapper;
    private readonly IGalleryImageRepository _galleryRepo;
    private readonly IImageAppService _imageAppService;
    private readonly ILogger<RoomService> _logger;

    public RoomService(IRoomRepository roomRepo, IRoomMapper mapper, IGalleryImageRepository galleryRepo,
        IImageAppService imageAppService,
        ILogger<RoomService> logger)
    {
        _roomRepo = roomRepo;
        _mapper = mapper;
        _galleryRepo = galleryRepo;
        _imageAppService = imageAppService;
        _logger = logger;
        
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

    public async Task<ImageResponseDto> UploadRoomImageAsync(
    Guid roomId, 
    ImageUploadDto imageUploadDto, 
    CancellationToken ct = default)
    {
        // Validate room exists
        var room = await _roomRepo.GetByIdAsync(roomId, ct);
        if (room == null)
            throw new KeyNotFoundException($"Room with ID {roomId} not found.");

        // Validate file
        ValidateImageFile.Validate(imageUploadDto.File);

        // Upload to cloud storage
        var folder = $"rooms/{roomId}";
        using var stream = imageUploadDto.File.OpenReadStream();
        
        var uploadResult = await _imageAppService.UploadAsync(
            stream, 
            imageUploadDto.File.FileName, 
            folder
        );

        // Create gallery image entity
        var image = new GalleryImage
        {
            Id = Guid.NewGuid(),
            Path = uploadResult,
        };

        room.Gallery.Add(image);

        await _galleryRepo.AddAsync(image, ct);
        await _roomRepo.UpdateAsync(room, ct);
        
        _logger.LogInformation("Image uploaded successfully for room {RoomId}, image ID: {ImageId}", 
            roomId, image.Id);

        return MapToImageResponseDto.Map(image);
    }
}