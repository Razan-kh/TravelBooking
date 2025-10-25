using Riok.Mapperly.Abstractions;
using TravelBooking.Application.RecentlyVisited.Dtos;
using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Application.RecentlyVisited.Mappers;

[Mapper]
public partial class RecentlyVisitedHotelMapper : IRecentlyVisitedHotelMapper
{
    [MapProperty(nameof(HotelWithMinPrice.Hotel.Id), nameof(RecentlyVisitedHotelDto.Id))]
    [MapProperty(nameof(HotelWithMinPrice.Hotel.Name), nameof(RecentlyVisitedHotelDto.Name))]
    [MapProperty(nameof(HotelWithMinPrice.Hotel.ThumbnailUrl), nameof(RecentlyVisitedHotelDto.ThumbnailUrl))]
    [MapProperty(nameof(HotelWithMinPrice.CityName), nameof(RecentlyVisitedHotelDto.City))]
    [MapProperty(nameof(HotelWithMinPrice.Hotel.StarRating), nameof(RecentlyVisitedHotelDto.StarRating))]
    [MapProperty(nameof(HotelWithMinPrice.MinPrice), nameof(RecentlyVisitedHotelDto.MinPrice))]
    public partial RecentlyVisitedHotelDto ToRecentlyVisitedHotelDto(HotelWithMinPrice hotel);
}