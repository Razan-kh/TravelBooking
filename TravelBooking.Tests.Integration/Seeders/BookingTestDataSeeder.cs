using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TravelBooking.Domain.Bookings.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Infrastructure.Persistence.Repositories;
using Xunit;
using AutoFixture;
using AutoFixture.Xunit2;
using TravelBooking.Domain.Payments.Enums;
using TravelBooking.Domain.Payments.Entities;
using TravelBooking.Domain.Rooms.Enums;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Hotels.Enums;
using TravelBooking.Domain.Users.Enums;
using TravelBooking.Domain.Users.Entities;
using TravelBooking.Tests.Integration.Builders;
using TravelBooking.Tests.Integration.Models;

namespace TravelBooking.Tests.Integration.Seeders;

public class BookingTestDataSeeder
{
    private readonly AppDbContext _dbContext;
    private readonly TestDataBuilder _dataBuilder;

    public BookingTestDataSeeder(AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _dataBuilder = new TestDataBuilder();
    }

    public async Task<BookingTestData> SeedBasicTestDataAsync()
    {
        var userId = Guid.NewGuid();
        var hotelId = Guid.NewGuid();
        var roomCategoryId = Guid.NewGuid();
        var roomId = Guid.NewGuid();

        var user = _dataBuilder.CreateUser(userId);
        var hotel = _dataBuilder.CreateHotel(hotelId);
        var roomCategory = _dataBuilder.CreateRoomCategory(hotelId, roomCategoryId);
        var room = _dataBuilder.CreateRoom(roomCategoryId, roomId);
        var booking = _dataBuilder.CreateBooking(userId, hotelId);
        
        booking.Rooms.Add(room);

        await _dbContext.Users.AddAsync(user);
        await _dbContext.Hotels.AddAsync(hotel);
        await _dbContext.RoomCategories.AddAsync(roomCategory);
        await _dbContext.Rooms.AddAsync(room);
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();

        return new BookingTestData
        {
            UserId = userId,
            HotelId = hotelId,
            RoomCategoryId = roomCategoryId,
            RoomId = roomId,
            User = user,
            Hotel = hotel,
            RoomCategory = roomCategory,
            Room = room,
            Booking = booking
        };
    }

    public async Task<Booking> AddAdditionalBookingAsync(Guid userId, Guid hotelId, Action<Booking> customize = null)
    {
        var booking = _dataBuilder.CreateBooking(userId, hotelId);
        customize?.Invoke(booking);
        
        await _dbContext.Bookings.AddAsync(booking);
        await _dbContext.SaveChangesAsync();
        
        return booking;
    }
}