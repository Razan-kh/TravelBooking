using Microsoft.AspNetCore.Http;
using TravelBooking.Domain.Images.Dtos;
using TravelBooking.Domain.Images.Entities;
using  TravelBooking.Domain.Shared.Interfaces;

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