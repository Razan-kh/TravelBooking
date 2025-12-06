using Riok.Mapperly.Abstractions;
using TravelBooking.Application.FeaturedDeals.Dtos;
using TravelBooking.Domain.Hotels;

namespace TravelBooking.Application.FeaturedDeals.Mappers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.None)]
public partial class FeaturedHotelMapper : IFeaturedHotelMapper
{
    [MapProperty(nameof(HotelWithMinPrice.Hotel.Id), nameof(FeaturedHotelDto.Id))]
    [MapProperty(nameof(HotelWithMinPrice.Hotel.Name), nameof(FeaturedHotelDto.Name))]
    [MapProperty(nameof(HotelWithMinPrice.Hotel.ThumbnailUrl), nameof(FeaturedHotelDto.ThumbnailUrl))]
    [MapProperty(nameof(HotelWithMinPrice.CityName), nameof(FeaturedHotelDto.City))]
    [MapProperty(nameof(HotelWithMinPrice.Hotel.StarRating), nameof(FeaturedHotelDto.StarRating))]
    [MapProperty(nameof(HotelWithMinPrice.MinPrice), nameof(FeaturedHotelDto.OriginalPrice))]
    [MapProperty(nameof(HotelWithMinPrice.DiscountedPrice), nameof(FeaturedHotelDto.DiscountedPrice))]
    public partial FeaturedHotelDto ToFeaturedHotelDto(HotelWithMinPrice hotel);
}