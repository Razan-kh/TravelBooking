using Microsoft.AspNetCore.Http;
using  TravelBooking.Domain.Shared.Interfaces;

namespace TravelBooking.Application.Images.Servicies;

public interface IImageAppService
{
    Task<string> UploadAsync(Stream imageStream, string fileName, string folder);
    Task<bool> DeleteAsync(string publicId);
}