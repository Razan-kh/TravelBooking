using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using Sieve.Models;
using Sieve.Services;
using TravelBooking.Application.Queries;
using TravelBooking.Application.Searching.Servicies.Implementations;
using TravelBooking.Domain.Cities.Entities;
using TravelBooking.Domain.Hotels.Entities;
using TravelBooking.Domain.Amenities.Entities;
using TravelBooking.Domain.Hotels.Interfaces.Repositories;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Tests.Carts.TestHelpers;

namespace TravelBooking.Tests.Searching.Handlers;

public class HotelServiceTests
{
    private readonly IFixture _fixture = FixtureFactory.Create();

    private readonly Mock<IHotelRepository> _repo;
    private readonly Mock<ISieveProcessor> _sieve;
    private readonly HotelService _sut;

    public HotelServiceTests()
    {
        _fixture = (Fixture?)new Fixture().Customize(new AutoMoqCustomization());

        // FIX: Required members support
        _fixture.Customize<City>(c => c
            .With(x => x.Country, "DefaultCountry")
            .With(x => x.PostalCode, "00000"));

        _repo = new Mock<IHotelRepository>();
        _sieve = new Mock<ISieveProcessor>();

        // Sieve always returns whatever IQueryable it receives
        _sieve.Setup(s => s.Apply(
                It.IsAny<SieveModel?>(),
                It.IsAny<IQueryable<Hotel>>(),
                It.IsAny<object[]?>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<bool>()))
            .Returns((SieveModel? m,
                      IQueryable<Hotel> q,
                      object[]? data,
                      bool applyFiltering,
                      bool applySorting,
                      bool applyPagination) => q);

        _sut = new HotelService(_repo.Object, _sieve.Object);
    }

    // Helpers
    private void MockQuery(IQueryable<Hotel> hotels)
    {
        _repo.Setup(r => r.Query()).Returns(hotels);
        _repo.Setup(r => r.ExecutePagedQueryAsync(
                It.IsAny<IQueryable<Hotel>>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
             .ReturnsAsync((IQueryable<Hotel> q, int take, CancellationToken _) =>
                 q.Take(take).ToList());
    }

    // Keyword filter
    [Fact]
    public async Task SearchAsync_Should_Filter_By_Keyword()
    {
        var paris = new City { Name = "Paris", Country = "France", PostalCode = "75000" };
        var london = new City { Name = "London", Country = "UK", PostalCode = "SW1A" };

        var hotels = new List<Hotel>
        {
            new Hotel { Id = Guid.NewGuid(), Name = "Sunrise Hotel", City = paris, RoomCategories = { new RoomCategory { PricePerNight = 100m } }
 },
            new Hotel { Id = Guid.NewGuid(), Name = "Moon Hotel", City = london, RoomCategories = { new RoomCategory { PricePerNight = 100m } }
 }
        }.AsQueryable();

        MockQuery(hotels);

        var request = new SearchHotelsQuery { Keyword = "sun" };

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("Sunrise Hotel");
    }

    //  City Filter
    [Fact]
    public async Task SearchAsync_Should_Filter_By_CityId()
    {
        var paris = new City { Id = Guid.NewGuid(), Name = "Paris", Country = "France", PostalCode = "75000" };
        var london = new City { Id = Guid.NewGuid(), Name = "London", Country = "UK", PostalCode = "SW1A" };

        var hotels = new List<Hotel>
        {
            new Hotel { Id = Guid.NewGuid(), Name = "A", CityId = paris.Id, City = paris, RoomCategories = { new RoomCategory { PricePerNight = 100m } }
 },
            new Hotel { Id = Guid.NewGuid(), Name = "B", CityId = london.Id, City = london, RoomCategories = { new RoomCategory { PricePerNight = 100m } } }
        }.AsQueryable();

        MockQuery(hotels);

        var request = new SearchHotelsQuery { CityId = paris.Id };

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Data.Should().HaveCount(1);
        result.Data.First().City.Should().Be("Paris");
    }

    //  Price filter
    [Fact]
    public async Task SearchAsync_Should_Filter_By_Price_Range()
    {
        var hotels = new List<Hotel>
        {
            new Hotel {
                Id = Guid.NewGuid(),
                Name ="Cheap",
                RoomCategories = { new RoomCategory { PricePerNight = 50m } }
            },
            new Hotel {
                Id = Guid.NewGuid(),
                Name ="Mid",
                RoomCategories = { new RoomCategory { PricePerNight = 120m } }
            },
            new Hotel {
                Id = Guid.NewGuid(),
                Name ="Expensive",
                RoomCategories = { new RoomCategory { PricePerNight = 300m } }
            }
        }.AsQueryable();

        MockQuery(hotels);

        var request = new SearchHotelsQuery
        {
            MinPrice = 80,
            MaxPrice = 200
        };

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("Mid");
    }

    // Amenities filter
    [Fact]
    public async Task SearchAsync_Should_Filter_By_Amenities()
    {
        var hotels = new List<Hotel>
        {
            new Hotel {
                Name="H1",
                RoomCategories =
                {
                    new RoomCategory {
                        Amenities = { new Amenity{ Name = "wifi" }, new Amenity{ Name = "pool" } }
                    }
                }
            },
            new Hotel {
                Name="H2",
                RoomCategories =
                {
                    new RoomCategory {
                        Amenities = { new Amenity{ Name = "AC" } }
                    }
                }
            }
        }.AsQueryable();

        MockQuery(hotels);

        var request = new SearchHotelsQuery
        {
            Amenities = new[] { "wifi", "pool" }
        };

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Data.Should().HaveCount(1);
        result.Data.First().Name.Should().Be("H1");
    }

    // Room availability
    [Fact]
    public async Task SearchAsync_Should_Filter_By_Availability()
    {
        var roomId = Guid.NewGuid();

        var hotels = new List<Hotel>
        {
            new Hotel {
                Name="AvailableHotel",
                RoomCategories = { new RoomCategory {
                    Id = roomId, AdultsCapacity = 2, ChildrenCapacity = 1
                }}
            }
        }.AsQueryable();

        MockQuery(hotels);

        _repo.Setup(r => r.IsRoomCategoryBookedAsync(
            roomId,
            It.IsAny<DateOnly>(),
            It.IsAny<DateOnly>()))
            .ReturnsAsync(false);

        var request = new SearchHotelsQuery
        {
            Adults = 2,
            Children = 1,
            CheckIn = new DateOnly(2025, 1, 10),
            CheckOut = new DateOnly(2025, 1, 12)
        };

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        result.Data.Should().HaveCount(1);
    }

    // Curser paging
    [Fact]
    public async Task SearchAsync_Should_Return_NextCursor_When_More_Items_Exist()
    {
        var london = new City { Name = "London", Country = "UK", PostalCode = "SW1A" };

        var hotels = Enumerable.Range(1, 25)
            .Select(i => new Hotel
            {
                Id = Guid.NewGuid(),
                Name = $"Hotel {i}",
                RoomCategories = { new RoomCategory { PricePerNight = 100 } },
                City = london
            })
            .AsQueryable();

        MockQuery(hotels);

        var request = new SearchHotelsQuery
        {
            PageSize = 20
        };

        var result = await _sut.SearchAsync(request, CancellationToken.None);

        var nextCursor = result.Meta.GetType().GetProperty("NextCursor")!.GetValue(result.Meta);

        nextCursor.Should().NotBeNull();
    }

    // Sieve should be called
    [Fact]
    public async Task SearchAsync_Should_Call_SieveProcessor()
    {
        var london = new City { Name = "London", Country = "UK", PostalCode = "SW1A" };

        var hotels = new List<Hotel>
        {
            new Hotel { Id = Guid.NewGuid(), Name = "Test", City = london , RoomCategories = { new RoomCategory { PricePerNight = 100m } }
         } }.AsQueryable();

        MockQuery(hotels);

        var request = new SearchHotelsQuery();

        await _sut.SearchAsync(request, CancellationToken.None);

        _sieve.Verify(s => s.Apply(
                request.SieveModel,
                It.IsAny<IQueryable<Hotel>>(),
                It.IsAny<object[]>(),
                true, true, true),
            Times.Once);
    }
}