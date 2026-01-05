using TravelBooking.Application.Shared.Results;
using TravelBooking.Application.Discounts.Dtos;

public interface IDiscountService
{
Task<Result<IEnumerable<DiscountDto>>> GetAllAsync(Guid hotelId, Guid roomCategoryId, CancellationToken ct);
Task<Result<DiscountDto>> GetByIdAsync(Guid hotelId, Guid roomCategoryId, Guid id, CancellationToken ct);
Task<Result<DiscountDto>> CreateAsync(Guid hotelId, Guid roomCategoryId, CreateDiscountDto dto, CancellationToken ct);
Task<Result<DiscountDto>> UpdateAsync(Guid hotelId, Guid roomCategoryId, UpdateDiscountDto dto, CancellationToken ct);
Task<Result> DeleteAsync(Guid hotelId, Guid roomCategoryId, Guid id, CancellationToken ct);
}