   using Microsoft.AspNetCore.Http;
using  TravelBooking.Domain.Shared.Interfaces;

namespace TravelBooking.Application.Images.Servicies;

public class ValidateImageFile
{
    public static void Validate(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        if (file.Length > 10 * 1024 * 1024) // 10MB limit
            throw new ArgumentException("File size exceeds 10MB limit");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type. Allowed: jpg, jpeg, png, gif, webp");

        var allowedContentTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
        if (!allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
            throw new ArgumentException("Invalid file content type");
    }
}