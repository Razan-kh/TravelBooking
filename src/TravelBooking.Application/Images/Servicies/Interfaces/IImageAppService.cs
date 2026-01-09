namespace TravelBooking.Application.Images.Servicies.Interfaces;

public interface IImageAppService
{
    Task<string> UploadAsync(Stream imageStream, string fileName, string folder);
    Task<bool> DeleteAsync(string publicId);
}