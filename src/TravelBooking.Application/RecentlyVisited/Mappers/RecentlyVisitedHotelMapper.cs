using Riok.Mapperly.Abstractions;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.RecentlyVisited.Mappers;

[Mapper]
public partial class RecentlyVisitedHotelMapper : IRecentlyVisitedHotelMapper
{
    [MapProperty(nameof(Hotel.Id), nameof(RecentlyVisitedHotelDto.Id))]
    [MapProperty(nameof(Hotel.Name), nameof(RecentlyVisitedHotelDto.Name))]
    [MapProperty(nameof(Hotel.ThumbnailUrl), nameof(RecentlyVisitedHotelDto.ThumbnailUrl))]
    [MapProperty(nameof(Hotel.City.Name), nameof(RecentlyVisitedHotelDto.City))]
    [MapProperty(nameof(Hotel.StarRating), nameof(RecentlyVisitedHotelDto.StarRating))]
    public partial RecentlyVisitedHotelDto ToRecentlyVisitedHotelDto(Hotel hotel);
}