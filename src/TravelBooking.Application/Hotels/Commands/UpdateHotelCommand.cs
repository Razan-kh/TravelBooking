
using MediatR;
using TravelBooking.Application.Shared.Results;

namespace TravelBooking.Application.Hotels.Commands;

public record UpdateHotelCommand(Guid Id, string Name, Guid CityId, Guid OwnerId, string Location, int StarRate, int RoomNumber, string Email, string ThumbnailUrl, string Description, string PhoneNumber, int TotalRooms)
    : IRequest<Result>;