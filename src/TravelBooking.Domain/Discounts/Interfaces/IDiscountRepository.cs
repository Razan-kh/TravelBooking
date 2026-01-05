using TravelBooking.Domain.Discounts.Entities;

namespace TravelBooking.Domain.Discounts.Interfaces;

public interface IDiscountRepository
{
    Task<Discount?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IEnumerable<Discount>> GetAllByRoomCategoryAsync(Guid roomCategoryId, CancellationToken ct);
    Task AddAsync(Discount entity, CancellationToken ct);
    Task UpdateAsync(Discount entity, CancellationToken ct);
    Task DeleteAsync(Discount entity, CancellationToken ct);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct);
}