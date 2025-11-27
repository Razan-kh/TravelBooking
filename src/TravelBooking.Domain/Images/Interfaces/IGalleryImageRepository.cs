using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Domain.Images.interfaces;

public interface IGalleryImageRepository
{
    Task<GalleryImage?> GetByIdAsync(Guid id, CancellationToken ct);
   /// Task<List<GalleryImage>> GetByRoomIdAsync(Guid roomId, CancellationToken ct);
    Task AddAsync(GalleryImage image, CancellationToken ct);
    Task UpdateAsync(GalleryImage image, CancellationToken ct);
    Task DeleteAsync(GalleryImage image, CancellationToken ct);
  //  Task SetPrimaryImageAsync(Guid roomId, Guid imageId, CancellationToken ct);
}