using MediatR;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Owners.Entities;

namespace TravelBooking.Application.Hotels.Commands;

public record CreateHotelCommand(string Name, City City, Owner Owner, string Location, int StarRate, int RoomNumber)
    : IRequest<Result<Guid>>;