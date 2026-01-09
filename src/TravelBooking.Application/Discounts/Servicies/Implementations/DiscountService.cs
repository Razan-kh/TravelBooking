using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Discounts.Mappers.Interfaces;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Interfaces;
using TravelBooking.Domain.Rooms.Interfaces;

namespace TravelBooking.Application.Discounts.Servicies;

public class DiscountService : IDiscountService
{
    private readonly IDiscountRepository _discountRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly IDiscountMapper _mapper;

    public DiscountService(IDiscountRepository discountRepository,
                            IRoomRepository roomCategoryRepository,
                            IDiscountMapper mapper)
    {
        _discountRepository = discountRepository;
        _roomRepository = roomCategoryRepository;
        _mapper = mapper;
    }

    public async Task<Result<IEnumerable<DiscountDto>>> GetAllAsync(Guid hotelId, Guid roomCategoryId, CancellationToken ct)
    {
        var roomCategory = await _roomRepository.GetRoomCategoryByIdAsync(roomCategoryId, ct);
        if (roomCategory is null)
            return Result<IEnumerable<DiscountDto>>.NotFound($"Room category with ID '{roomCategoryId}' was not found.");
        
        if (roomCategory.HotelId != hotelId)
            return Result<IEnumerable<DiscountDto>>.Forbidden($"Room category does not belong to hotel with ID '{hotelId}'.");

        var discounts = await _discountRepository.GetAllByRoomCategoryAsync(roomCategoryId, ct);
        var dtos = discounts.Select(_mapper.ToDto).ToList();
        
        return Result<IEnumerable<DiscountDto>>.Success(dtos);
    }

    public async Task<Result<DiscountDto>> GetByIdAsync(Guid hotelId, Guid roomCategoryId, Guid id, CancellationToken ct)
    {
        var roomCategory = await _roomRepository.GetRoomCategoryByIdAsync(roomCategoryId, ct);
        if (roomCategory is null)
            return Result<DiscountDto>.NotFound($"Room category with ID '{roomCategoryId}' was not found.");
        
        if (roomCategory.HotelId != hotelId)
            return Result<DiscountDto>.Forbidden($"Room category does not belong to hotel with ID '{hotelId}'.");

        var discount = await _discountRepository.GetByIdAsync(id, ct);
        if (discount is null)
            return Result<DiscountDto>.NotFound($"Discount with ID '{id}' was not found.");
        
        if (discount.RoomCategoryId != roomCategoryId)
            return Result<DiscountDto>.NotFound($"Discount with ID '{id}' was not found for room category with ID '{roomCategoryId}'.");

        return Result<DiscountDto>.Success(_mapper.ToDto(discount));
    }

    public async Task<Result<DiscountDto>> CreateAsync(Guid hotelId, Guid roomCategoryId, CreateDiscountDto dto, CancellationToken ct)
    {
        var roomCategory = await _roomRepository.GetRoomCategoryByIdAsync(roomCategoryId, ct);
        if (roomCategory is null)
            return Result<DiscountDto>.NotFound($"Room category with ID '{roomCategoryId}' was not found.");
        
        if (roomCategory.HotelId != hotelId)
            return Result<DiscountDto>.Forbidden($"Room category does not belong to hotel with ID '{hotelId}'.");

        // Validation (should be done by a validator, but showing here for completeness)
        if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
            return Result<DiscountDto>.ValidationError("Discount percentage must be between 0 and 100.");
        
        if (dto.EndDate <= dto.StartDate)
            return Result<DiscountDto>.ValidationError("End date must be after start date.");

        var discount = new Discount
        {
            Id = Guid.NewGuid(),
            DiscountPercentage = dto.DiscountPercentage,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            RoomCategoryId = roomCategoryId
        };

        await _discountRepository.AddAsync(discount, ct);
        return Result<DiscountDto>.Success(_mapper.ToDto(discount), 201); 
    }

    public async Task<Result<DiscountDto>> UpdateAsync(Guid hotelId, Guid roomCategoryId, UpdateDiscountDto dto, CancellationToken ct)
    {
        var roomCategory = await _roomRepository.GetRoomCategoryByIdAsync(roomCategoryId, ct);
        if (roomCategory is null)
            return Result<DiscountDto>.NotFound($"Room category with ID '{roomCategoryId}' was not found.");
        
        if (roomCategory.HotelId != hotelId)
            return Result<DiscountDto>.Forbidden($"Room category does not belong to hotel with ID '{hotelId}'.");

        var discount = await _discountRepository.GetByIdAsync(dto.Id, ct);
        if (discount is null)
            return Result<DiscountDto>.NotFound($"Discount with ID '{dto.Id}' was not found.");
        
        if (discount.RoomCategoryId != roomCategoryId)
            return Result<DiscountDto>.NotFound($"Discount with ID '{dto.Id}' was not found for room category with ID '{roomCategoryId}'.");

        // Validation
        if (dto.DiscountPercentage <= 0 || dto.DiscountPercentage > 100)
            return Result<DiscountDto>.ValidationError("Discount percentage must be between 0 and 100.");
        
        if (dto.EndDate <= dto.StartDate)
            return Result<DiscountDto>.ValidationError("End date must be after start date.");

        discount.DiscountPercentage = dto.DiscountPercentage;
        discount.StartDate = dto.StartDate;
        discount.EndDate = dto.EndDate;

        await _discountRepository.UpdateAsync(discount, ct);
        return Result<DiscountDto>.Success(_mapper.ToDto(discount));
    }

    public async Task<Result> DeleteAsync(Guid hotelId, Guid roomCategoryId, Guid id, CancellationToken ct)
    {
        var roomCategory = await _roomRepository.GetRoomCategoryByIdAsync(roomCategoryId, ct);
        if (roomCategory is null)
            return Result.NotFound($"Room category with ID '{roomCategoryId}' was not found.");
        
        if (roomCategory.HotelId != hotelId)
            return Result.Forbidden($"Room category does not belong to hotel with ID '{hotelId}'.");

        var discount = await _discountRepository.GetByIdAsync(id, ct);
        if (discount is null)
            return Result.NotFound($"Discount with ID '{id}' was not found.");
        
        if (discount.RoomCategoryId != roomCategoryId)
            return Result.NotFound($"Discount with ID '{id}' was not found for room category with ID '{roomCategoryId}'.");

        await _discountRepository.DeleteAsync(discount, ct);
        return Result.Success();
    }
}