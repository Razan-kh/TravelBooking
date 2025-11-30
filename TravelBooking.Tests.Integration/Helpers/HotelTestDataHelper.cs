using TravelBooking.Domain.Hotels.Entities;

namespace TravelBooking.Tests.Integration.Admin.Helpers;

public static class HotelTestDataHelper
{
    public static class HotelNames
    {
        public const string HotelA = "A Hotel";
        public const string HotelB = "B Hotel"; 
        public const string HotelX = "HotelX";
        public const string IntegrationHotel = "Integration Hotel";
        public const string UpdatedHotel = "Updated Hotel";
        public const string OldHotel = "Old Hotel";
    }

    public static class TestEmails
    {
        public const string HotelEmail = "hotel@test.com";
        public const string DefaultEmail = "a@b.com";
    }

    public static class TestLocations
    {
        public const string DefaultLocation = "Test Location";
        public const string SimpleLocation = "loc";
    }
}