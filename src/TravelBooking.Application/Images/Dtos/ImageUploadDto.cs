using Microsoft.AspNetCore.Http;

namespace TravelBooking.Application.Images.Dtos;

public record ImageUploadDto(
    IFormFile File,
    string? AltText = null,
    int DisplayOrder = 0,
    bool IsPrimary = false
);