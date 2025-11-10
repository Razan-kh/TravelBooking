using AutoMapper;
using TravelBooking.Application.DTOs;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Images.Entities;
using TravelBooking.Domain.Reviews.Entities;
using TravelBooking.Domain.Rooms.Entities;

namespace TravelBooking.Application.ViewingHotels.Mappers;

public class HotelMappingProfile : Profile
{
    public HotelMappingProfile()
    {
        CreateMap<Hotel, HotelDetailsDto>()
            .ForMember(d => d.City, o => o.MapFrom(s => s.City != null ? s.City.Name : string.Empty))
            .ForMember(d => d.Gallery, o => o.MapFrom(s => s.Gallery));

        CreateMap<GalleryImage, GalleryImageDto>();
        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.UserName, o => o.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : null));
        CreateMap<RoomCategory, RoomCategoryDto>()
            .ForMember(d => d.Amenities, o => o.MapFrom(s => s.Amenities.Select(a => a.Name)))
            .ForMember(d => d.Gallery, o => o.MapFrom(s => s.Rooms.SelectMany(r => r.Gallery).Distinct())); // example
    }
}