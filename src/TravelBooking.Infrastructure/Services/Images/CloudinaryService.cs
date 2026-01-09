using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using TravelBooking.Domain.Shared.Interfaces;

namespace TravelBooking.Infrastructure.Services.Images;

public class CloudinaryService : ICloudStorageService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService(IOptions<CloudinarySettings> config)
    {
        var settings = config.Value;
        Account account = new Account(settings.CloudName, settings.ApiKey, settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string folder)
    {

        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(fileName, imageStream),
            Folder = folder,
        };

        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.ToString();
    }

    public async Task<bool> DeleteImageAsync(string publicId)
    {
        var delParams = new DeletionParams(publicId);

        var result = await _cloudinary.DestroyAsync(delParams);
        return result.Result == "ok";
    }
}