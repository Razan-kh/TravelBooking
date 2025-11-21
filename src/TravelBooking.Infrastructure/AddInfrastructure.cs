using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Domain.Bookings.Repositories;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Carts.Repositories;
using TravelBooking.Domain.Rooms.Repositories;
using TravelBooking.Application.Shared.Interfaces;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Domain.Reviews.Repositories;
using TravelBooking.Domain.Users.Repositories;
using TravelBooking.Infrastructure.Persistence.Repositories;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;

namespace TravelBooking.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static void AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

    }
}
