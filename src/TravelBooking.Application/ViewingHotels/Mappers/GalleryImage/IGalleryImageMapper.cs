using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

public interface IGalleryImageMapper
{
    GalleryImageDto Map(GalleryImage galleryImage);
}