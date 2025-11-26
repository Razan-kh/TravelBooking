using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TravelBooking.Api.Carts.Controllers;
using TravelBooking.Application.AddingToCart.Commands;
using TravelBooking.Application.Carts.DTOs;
using TravelBooking.Application.Shared.Results;
using TravelBooking.Domain.Carts.Entities;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Infrastructure.Persistence;
using TravelBooking.Tests.Integration.Extensions;
using TravelBooking.Tests.Integration.Helpers;
using Xunit;
/*  ///REMOVE THIS ------------------------------
namespace TravelBooking.Tests.Integration.Controllers
{
    public class CartControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
                private readonly DbContextOptions<AppDbContext> _dbContextOptions;

        private  AppDbContext _dbContext;
        private readonly Fixture _fixture;
        private readonly WebApplicationFactory<Program> _factory;

        public CartControllerTests(WebApplicationFactory<Program> factory)
        {
            _fixture = new Fixture();
            _factory = factory;
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Get the DbContext from the test server
            var scope = factory.Services.CreateScope();
                _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"TestDatabase_{Guid.NewGuid()}") // Unique name for each test
            .Options;

        _dbContext = new AppDbContext(_dbContextOptions);
        //    _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//          _dbContext = _factory.Services.GetRequiredService<AppDbContext>();

        }
        /*
        public async Task InitializeAsync()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("YourTestConnectionString")
                .Options;

            _dbContext = new AppDbContext(options);

            // Ensure database is created and migrations applied
            await _dbContext.Database.EnsureDeletedAsync(); // Clean start
            await _dbContext.Database.EnsureCreatedAsync();
        }
        /*
                [Fact]
                public async Task AddRoomToCart_ValidRequest_ReturnsOk()
                {
                    // Arrange
                    var testUserId = Guid.NewGuid();
                    var roomCategoryId = Guid.NewGuid();

                    var command = new AddRoomToCartCommand(
                        roomCategoryId, 
                        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                        DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                        2)
                    {
                        UserId = testUserId
                    };

                    var client = _factory.CreateClientWithUser(testUserId, "user"); // Use testUserId consistently

                    // Act
                    var response = await client.PostAsJsonAsync("/api/cart", command);

                    // Assert
                    response.StatusCode.Should().Be(HttpStatusCode.OK);

                    // Verify cart item is persisted - include Items and use proper query
                    var cart = await _dbContext.Carts
                        .Include(c => c.Items) // Important: include related data
                        .FirstOrDefaultAsync(c => c.UserId == testUserId); // Use FirstOrDefaultAsync instead of FindAsync

                    cart.Should().NotBeNull();
                    cart!.Items.Should().ContainSingle(i =>
                        i.RoomCategoryId == roomCategoryId &&
                        i.Quantity == 2);
                }
        */
        /*  ///REMOVE THIS ------------------------------

        [Fact]
        public async Task AddRoomToCart_CheckOutBeforeCheckIn_ReturnsBadRequest()
        {
            // Arrange
            var testUserId = Guid.NewGuid();

            var command = new AddRoomToCartCommand(
                Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)), // CheckIn later
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)), // CheckOut earlier - invalid!
                2
            );

            var client = _factory.CreateClientWithUser(testUserId, "user");

            // Act
            var response = await client.PostAsJsonAsync("/api/cart", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCart_WithItems_ReturnsOkWithCartItems()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var roomCategory = _fixture.CreateRoomCategoryMinimal();
            var checkIn = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)); // CheckIn later
            var checkOut = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(4));

            var cartItems = _fixture.Build<CartItem>()
                        .With(x => x.Quantity, 3)
                        .With(x => x.RoomCategory, roomCategory)
                        .With(x => x.CheckIn, checkIn)
                        .With(x => x.CheckOut, checkOut)
                        .Without(x => x.Cart)
                        .With(x => x.RoomCategoryId, roomCategory.Id)
                        .Create();
            // Seed cart
            var cart = new Cart
            {
                UserId = testUserId,
                Items = new List<CartItem>
                {
                    cartItems
                }
            };
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();

            var client = _factory.CreateClientWithUser(testUserId, "user"); // Use same testUserId

            // Act
            var response = await client.GetAsync("/api/cart");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Result<List<CartItemDto>>>();
            result.Should().NotBeNull();
            result!.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task RemoveItem_ExistingCartItem_ReturnsOk()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var cartItemId = Guid.NewGuid();

            var cart = new Cart
            {
                UserId = testUserId,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        Id = cartItemId,
                        RoomCategoryId = Guid.NewGuid(),
                        Quantity = 1
                    }
                }
            };

            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();

            var client = _factory.CreateClientWithUser(testUserId, "user");

            // Act
            var response = await client.DeleteAsync($"/api/cart/{cartItemId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var updatedCart = await _dbContext.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == testUserId);

            updatedCart!.Items.Should().BeEmpty();
        }
/*
        [Fact]
        public async Task RemoveItem_NonExistingCartItem_ReturnsNotFound()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var nonExistingId = Guid.NewGuid();

            var client = _factory.CreateClientWithUser(testUserId, "user");

            // Act
            var response = await client.DeleteAsync($"/api/cart/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        */
        /*  ///REMOVE THIS ------------------------------

    }
}
*/ /*  ///REMOVE THIS ------------------------------

//----------------------------------------------
/*
namespace TravelBooking.Tests.Integration.Controllers
{
    public class CartControllerTests : IClassFixture<ApiTestFactory>
    {
        private readonly HttpClient _client;
        private readonly AppDbContext _dbContext;
        private readonly Fixture _fixture;

        public CartControllerTests(ApiTestFactory factory)
        {
          //  _client = factory.CreateClient();
            _fixture = new Fixture();
        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            // Get the DbContext from the test server
            var scope = factory.Services.CreateScope();
            _dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        }

        [Fact]
        public async Task AddRoomToCart_ValidRequest_ReturnsOk()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var roomCategoryId = Guid.NewGuid();
            var command = new AddRoomToCartCommand
            (
            Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                2
            );

    //  _client.DefaultRequestHeaders.Add("Test-User", testUserId.ToString());
        var client = _factory.CreateClientWithRole("user");

            // Act
            var response = await _client.PostAsJsonAsync("/api/cart", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            // Verify cart item is persisted
            var cart = await _dbContext.Carts
                .FindAsync(testUserId);

            cart.Should().NotBeNull();
            cart!.Items.Should().ContainSingle(i =>
                i.RoomCategoryId == roomCategoryId &&
                i.Quantity == 2);
        }

        [Fact]
        public async Task AddRoomToCart_CheckOutBeforeCheckIn_ReturnsBadRequest()
        {
            // Arrange
            var testUserId = Guid.NewGuid();

            var command = new AddRoomToCartCommand(
            Guid.NewGuid(),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)),
                DateOnly.FromDateTime(DateTime.UtcNow.AddDays(3)),
                2
            );
            _client.DefaultRequestHeaders.Add("Test-User", testUserId.ToString());

            // Act
            var response = await _client.PostAsJsonAsync("/api/cart", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetCart_WithItems_ReturnsOkWithCartItems()
        {
            // Arrange
            var testUserId = Guid.NewGuid();

            // Seed cart
            var cart = new Cart
            {
                UserId = testUserId,
                Items = new List<CartItem>
                {
                    _fixture.Build<CartItem>()
                        .With(x => x.RoomCategoryId, Guid.NewGuid())
                        .With(x => x.Quantity, 3)
                        .Create()
                }
            };
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();

            _client.DefaultRequestHeaders.Add("Test-User", testUserId.ToString());

            // Act
            var response = await _client.GetAsync("/api/cart");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<Result<List<CartItemDto>>>();
            result.Should().NotBeNull();
            result!.Value.Should().HaveCount(1);
        }

        [Fact]
        public async Task RemoveItem_ExistingCartItem_ReturnsOk()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            var cartItem = _fixture.Build<CartItem>()
                                   .With(x => x.CartId, Guid.NewGuid())
                                   .Create();

            var cart = new Cart
            {
                UserId = testUserId,
                Items = new List<CartItem> { cartItem }
            };
            _dbContext.Carts.Add(cart);
            await _dbContext.SaveChangesAsync();

            _client.DefaultRequestHeaders.Add("Test-User", testUserId.ToString());

            // Act
            var response = await _client.DeleteAsync($"/api/cart/{cartItem.Id}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var updatedCart = await _dbContext.Carts
                .FindAsync(testUserId);
            updatedCart!.Items.Should().BeEmpty();
        }

        [Fact]
        public async Task RemoveItem_NonExistingCartItem_ReturnsNotFound()
        {
            // Arrange
            var testUserId = Guid.NewGuid();
            _client.DefaultRequestHeaders.Add("Test-User", testUserId.ToString());
            var nonExistingId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/cart/{nonExistingId}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
*/