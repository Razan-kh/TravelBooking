using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Images.interfaces;
using TravelBooking.Infrastructure.Persistence;

namespace TravelBooking.Infrastructure.Persistence.Repositories;

public class GalleryImageRepository : IGalleryImageRepository
{
    private readonly AppDbContext _context;

    public GalleryImageRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<GalleryImage?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await _context.GalleryImages
            .FirstOrDefaultAsync(img => img.Id == id, ct);
    }

    public async Task<List<GalleryImage>> GetByRoomIdAsync(Guid roomId, CancellationToken ct)
    {
        return await _context.GalleryImages
            .Where(img => img.RoomId == roomId)
            .ToListAsync(ct);
    }

    public async Task AddAsync(GalleryImage image, CancellationToken ct)
    {
        await _context.GalleryImages.AddAsync(image, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(GalleryImage image, CancellationToken ct)
    {
        _context.GalleryImages.Update(image);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(GalleryImage image, CancellationToken ct)
    {
        _context.GalleryImages.Remove(image);
        await _context.SaveChangesAsync(ct);
    }
/*
    public async Task SetPrimaryImageAsync(Guid roomId, Guid imageId, CancellationToken ct)
    {
        // Reset all images to non-primary
        var roomImages = await _context.GalleryImages
            .Where(img => img.RoomId == roomId)
            .ToListAsync(ct);

        foreach (var image in roomImages)
        {
            image.IsPrimary = image.Id == imageId;
        }

        await _context.SaveChangesAsync(ct);
    }
    */
}