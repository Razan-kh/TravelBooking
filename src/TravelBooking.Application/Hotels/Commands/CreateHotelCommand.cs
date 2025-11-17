using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Commands;

public class CreateHotelCommand : IRequest<Result<Guid>>
{
    public string Name { get; set; }
    public Guid CityId { get; set; }
    public Guid OwnerId { get; set; }
    public string Location { get; set; }
    public int StarRate { get; set; }
    public int RoomNumber { get; set; }
    public string Description { get; set; }
    public string Email { get; set; }
    public string ThumbnailUrl { get; set; }
}