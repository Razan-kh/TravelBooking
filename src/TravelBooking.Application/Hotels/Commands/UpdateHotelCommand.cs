
using MediatR;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Cities;
using TravelBooking.Domain.Owners.Entities;

namespace TravelBooking.Application.Hotels.Commands;

public record UpdateHotelCommand(Guid Id, string Name, Guid City, Guid Owner, string Location, int StarRate, int RoomNumber)
    : IRequest<Result>;