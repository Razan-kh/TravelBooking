using TravelBooking.Application.Images.DTOs;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Images.Mappers.Interfaces;

public interface IGalleryImageMapper
{
    GalleryImageDto Map(GalleryImage galleryImage);
}