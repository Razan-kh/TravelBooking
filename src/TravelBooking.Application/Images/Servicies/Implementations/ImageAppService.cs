using TravelBooking.Application.Images.Servicies.Interfaces;
using  TravelBooking.Domain.Shared.Interfaces;

namespace TravelBooking.Application.Images.Servicies.Implementations;

public class ImageAppService : IImageAppService
{
    private readonly ICloudStorageService _cloudStorage;

    public ImageAppService(ICloudStorageService cloudStorage)
    {
        _cloudStorage = cloudStorage;
    }

    public async Task<string> UploadAsync(Stream imageStream, string fileName, string folder)
    {
        return await _cloudStorage.UploadImageAsync(imageStream, fileName, folder);
    }

    public async Task<bool> DeleteAsync(string publicId)
    {
        return await _cloudStorage.DeleteImageAsync(publicId);
    }
}