namespace TravelBooking.Domain.Shared.Interfaces;

public interface ICloudStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder);
    Task<bool> DeleteImageAsync(string publicId);
}
