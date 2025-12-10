using Riok.Mapperly.Abstractions;
using TravelBooking.Application.Images.DTOs;
using TravelBooking.Application.Images.Mappers.Interfaces;
using TravelBooking.Domain.Images.Entities;

namespace TravelBooking.Application.Images.Mappers.Implementations;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class GalleryImageMapper : IGalleryImageMapper
{
    public partial GalleryImageDto Map(GalleryImage galleryImage);

}