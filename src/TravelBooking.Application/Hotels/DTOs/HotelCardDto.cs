using System;
using System.Collections.Generic;

namespace YourNamespace.Application.DTOs;

public class HotelCardDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string City { get; set; } = string.Empty;
    public int StarRating { get; set; }
    public decimal? MinPrice { get; set; }
    public IEnumerable<string> Amenities { get; set; } = new List<string>();
}