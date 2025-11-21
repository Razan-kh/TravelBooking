using Riok.Mapperly.Abstractions;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

[Mapper]
public partial class GalleryImageMapper : IGalleryImageMapper
{
    public partial GalleryImageDto Map(GalleryImage galleryImage);

}