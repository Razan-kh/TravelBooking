using Microsoft.AspNetCore.Http;
using TravelBooking.Application.Images.Dtos;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Images.Servicies;

public class MapToImageResponseDto
{
    public static ImageResponseDto Map(GalleryImage image)
    {
        return new ImageResponseDto(
            image.Id,
            image.Path
        );
    }
}