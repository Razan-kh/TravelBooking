using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Discounts.Dtos;
using TravelBooking.Application.Discounts.Mappers.Implementations;
using TravelBooking.Application.Discounts.Mappers.Interfaces;
using TravelBooking.Application.Discounts.Servicies;
using TravelBooking.Domain.Discounts.Entities;
using TravelBooking.Domain.Discounts.Interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Interfaces;

namespace TravelBooking.Tests.Unit.Discounts.Servicies;

public class DiscountServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IDiscountRepository> _discountRepositoryMock;
    private readonly Mock<IRoomRepository> _roomRepositoryMock;
    private readonly Mock<IDiscountMapper> _mapperMock;
    private readonly DiscountService _sut;
    private readonly CancellationToken _cancellationToken;

    public DiscountServiceTests()
    {
        _fixture = new Fixture()
            .Customize(new AutoMoqCustomization());

        _discountRepositoryMock = _fixture.Freeze<Mock<IDiscountRepository>>();
        _roomRepositoryMock = _fixture.Freeze<Mock<IRoomRepository>>();
        _mapperMock = _fixture.Freeze<Mock<IDiscountMapper>>();

        _sut = new DiscountService(
            _discountRepositoryMock.Object,
            _roomRepositoryMock.Object,
            _mapperMock.Object);

        _cancellationToken = CancellationToken.None;
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_WhenRoomCategoryNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync((RoomCategory)null);

        // Act
        var result = await _sut.GetAllAsync(hotelId, roomCategoryId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Room category with ID '{roomCategoryId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");

        _discountRepositoryMock.Verify(
            x => x.GetAllByRoomCategoryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WhenRoomCategoryBelongsToDifferentHotel_ReturnsForbiddenResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var differentHotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, differentHotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        // Act
        var result = await _sut.GetAllAsync(hotelId, roomCategoryId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(403);
        result.Error.Should().Be($"Room category does not belong to hotel with ID '{hotelId}'.");
        result.ErrorCode.Should().Be("FORBIDDEN");

        _discountRepositoryMock.Verify(
            x => x.GetAllByRoomCategoryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoDiscountsExist_ReturnsEmptyList()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        _discountRepositoryMock
            .Setup(x => x.GetAllByRoomCategoryAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(new List<Discount>());

        // Act
        var result = await _sut.GetAllAsync(hotelId, roomCategoryId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();

        _discountRepositoryMock.Verify(
            x => x.GetAllByRoomCategoryAsync(roomCategoryId, _cancellationToken),
            Times.Once);
    }

    #endregion

    #region GetByIdAsync Tests

    [Fact]
    public async Task GetByIdAsync_WhenRoomCategoryNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(default(RoomCategory));

        // Act
        var result = await _sut.GetByIdAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Room category with ID '{roomCategoryId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");
        
        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenRoomCategoryBelongsToDifferentHotel_ReturnsForbiddenResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var differentHotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();
        
        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, differentHotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        // Act
        var result = await _sut.GetByIdAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(403);
        result.Error.Should().Be($"Room category does not belong to hotel with ID '{hotelId}'.");
        result.ErrorCode.Should().Be("FORBIDDEN");
        
        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDiscountNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();
        
        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);
        
        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(discountId, _cancellationToken))
            .ReturnsAsync(default(Discount));

        // Act
        var result = await _sut.GetByIdAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Discount with ID '{discountId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");
        
        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(discountId, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenDiscountBelongsToDifferentRoomCategory_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var differentRoomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        var discount = _fixture.Build<Discount>()
            .With(d => d.Id, discountId)
            .With(d => d.RoomCategoryId, differentRoomCategoryId)
            .Without(d => d.RoomCategory)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(discountId, _cancellationToken))
            .ReturnsAsync(discount);

        // Act
        var result = await _sut.GetByIdAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Discount with ID '{discountId}' was not found for room category with ID '{roomCategoryId}'.");
        result.ErrorCode.Should().Be("NOT_FOUND");

        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(discountId, _cancellationToken),
            Times.Once);
    }
    
    [Fact]
    public async Task GetByIdAsync_WhenValidRequest_ReturnsMappedDiscount()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        var discount = _fixture.Build<Discount>()
            .With(d => d.Id, discountId)
            .With(d => d.RoomCategoryId, roomCategoryId)
            .Without(d => d.RoomCategory)
            .Create();

        var expectedDto = new DiscountDto(
            discount.Id,                 
            discount.DiscountPercentage,
            discount.StartDate,
            discount.EndDate,
            roomCategoryId
        );

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(discountId, _cancellationToken))
            .ReturnsAsync(discount);

        // Mock the mapper instead of using the real one
        _mapperMock
            .Setup(m => m.ToDto(It.Is<Discount>(d => d.Id == discount.Id)))
            .Returns(expectedDto);

        // Act
        var result = await _sut.GetByIdAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expectedDto);

        _discountRepositoryMock.Verify(x => x.GetByIdAsync(discountId, _cancellationToken), Times.Once);
        _mapperMock.Verify(x => x.ToDto(discount), Times.Once);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_WhenRoomCategoryNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var createDiscountDto = _fixture.Create<CreateDiscountDto>();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(default(RoomCategory));

        // Act
        var result = await _sut.CreateAsync(hotelId, roomCategoryId, createDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Room category with ID '{roomCategoryId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");
        
        _discountRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Discount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenRoomCategoryBelongsToDifferentHotel_ReturnsForbiddenResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var differentHotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var createDiscountDto = _fixture.Create<CreateDiscountDto>();
        
        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, differentHotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        // Act
        var result = await _sut.CreateAsync(hotelId, roomCategoryId, createDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(403);
        result.Error.Should().Be($"Room category does not belong to hotel with ID '{hotelId}'.");
        result.ErrorCode.Should().Be("FORBIDDEN");
        
        _discountRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Discount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    [InlineData(101)]
    [InlineData(150)]
    public async Task CreateAsync_WhenInvalidDiscountPercentage_ReturnsValidationError(decimal invalidPercentage)
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var createDiscountDto = _fixture.Build<CreateDiscountDto>()
            .With(d => d.DiscountPercentage, invalidPercentage)
            .Create();
        
        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        // Act
        var result = await _sut.CreateAsync(hotelId, roomCategoryId, createDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(400);
        result.Error.Should().Be("Discount percentage must be between 0 and 100.");
        result.ErrorCode.Should().Be("VALIDATION_ERROR");
        
        _discountRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Discount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenEndDateIsBeforeStartDate_ReturnsValidationError()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var startDate = DateTime.UtcNow.AddDays(10);
        var endDate = DateTime.UtcNow.AddDays(5); // End date before start date

        var createDiscountDto = _fixture.Build<CreateDiscountDto>()
            .With(d => d.StartDate, startDate)
            .With(d => d.EndDate, endDate)
            .With(d => d.DiscountPercentage, 20)
            .Create();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        // Act
        var result = await _sut.CreateAsync(hotelId, roomCategoryId, createDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(400);
        result.Error.Should().Be("End date must be after start date.");
        result.ErrorCode.Should().Be("VALIDATION_ERROR");

        _discountRepositoryMock.Verify(
            x => x.AddAsync(It.IsAny<Discount>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
    
    [Fact]
    public async Task CreateAsync_WhenValidRequest_CreatesAndReturnsDiscountWith201Created()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var createDiscountDto = _fixture.Build<CreateDiscountDto>()
            .With(d => d.DiscountPercentage, 20)
            .With(d => d.StartDate, DateTime.UtcNow.AddDays(1))
            .With(d => d.EndDate, DateTime.UtcNow.AddDays(10))
            .Create();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        Discount capturedDiscount = null;
        _discountRepositoryMock
            .Setup(x => x.AddAsync(It.IsAny<Discount>(), _cancellationToken))
            .Callback<Discount, CancellationToken>((d, _) => capturedDiscount = d)
            .Returns(Task.CompletedTask);

        // Create the expected DiscountDto from the captured discount
    _mapperMock
        .Setup(m => m.ToDto(It.IsAny<Discount>()))
        .Returns((Discount d) => new DiscountDto(
            d.Id,
            d.DiscountPercentage,
            d.StartDate,
            d.EndDate,
            d.RoomCategoryId
        ));


        // Act
        var result = await _sut.CreateAsync(hotelId, roomCategoryId, createDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.HttpStatusCode.Should().Be(201);

        // Verify the captured discount matches the input
        capturedDiscount.Should().NotBeNull();
        capturedDiscount!.Id.Should().NotBeEmpty();
        capturedDiscount.RoomCategoryId.Should().Be(roomCategoryId);
        capturedDiscount.DiscountPercentage.Should().Be(createDiscountDto.DiscountPercentage);
        capturedDiscount.StartDate.Should().Be(createDiscountDto.StartDate);
        capturedDiscount.EndDate.Should().Be(createDiscountDto.EndDate);

        // Verify the returned DTO matches the mapped discount
        var expectedDto = new DiscountDto(
            capturedDiscount.Id,
            capturedDiscount.DiscountPercentage,
            capturedDiscount.StartDate,
            capturedDiscount.EndDate,
            capturedDiscount.RoomCategoryId
        );
        result.Value.Should().BeEquivalentTo(expectedDto);

        _discountRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Discount>(d => d.RoomCategoryId == roomCategoryId), _cancellationToken),
            Times.Once
        );

        _mapperMock.Verify(x => x.ToDto(It.IsAny<Discount>()), Times.Once);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_WhenRoomCategoryNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var updateDiscountDto = _fixture.Create<UpdateDiscountDto>();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync((RoomCategory)null);

        // Act
        var result = await _sut.UpdateAsync(hotelId, roomCategoryId, updateDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Room category with ID '{roomCategoryId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");
        
        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenDiscountNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var updateDiscountDto = _fixture.Create<UpdateDiscountDto>();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDiscountDto.Id, _cancellationToken))
            .ReturnsAsync((Discount)null);

        // Act
        var result = await _sut.UpdateAsync(hotelId, roomCategoryId, updateDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Discount with ID '{updateDiscountDto.Id}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");

        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(updateDiscountDto.Id, _cancellationToken),
            Times.Once);
    }
    [Fact]
    public async Task UpdateAsync_WhenValidRequest_UpdatesAndReturnsDiscount()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var updateDiscountDto = _fixture.Build<UpdateDiscountDto>()
            .With(d => d.DiscountPercentage, 25)
            .With(d => d.StartDate, DateTime.UtcNow.AddDays(1))
            .With(d => d.EndDate, DateTime.UtcNow.AddDays(15))
            .Create();

        var existingDiscount = _fixture.Build<Discount>()
            .With(d => d.Id, updateDiscountDto.Id)
            .With(d => d.RoomCategoryId, roomCategoryId)
            .With(d => d.DiscountPercentage, 10) // original value
            .With(d => d.StartDate, DateTime.UtcNow.AddDays(-5))
            .With(d => d.EndDate, DateTime.UtcNow.AddDays(5))
            .Without(d => d.RoomCategory)
            .Create();

        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();

        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);

        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(updateDiscountDto.Id, _cancellationToken))
            .ReturnsAsync(existingDiscount);

        // Mock the mapper interface
        var updatedDiscountDto = _fixture.Create<DiscountDto>();
        _mapperMock
            .Setup(m => m.ToDto(existingDiscount))
            .Returns(updatedDiscountDto);

        // Act
        var result = await _sut.UpdateAsync(hotelId, roomCategoryId, updateDiscountDto, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(updatedDiscountDto);

        existingDiscount.DiscountPercentage.Should().Be(updateDiscountDto.DiscountPercentage);
        existingDiscount.StartDate.Should().Be(updateDiscountDto.StartDate);
        existingDiscount.EndDate.Should().Be(updateDiscountDto.EndDate);

        _discountRepositoryMock.Verify(
            x => x.UpdateAsync(existingDiscount, _cancellationToken),
            Times.Once);

        _mapperMock.Verify(x => x.ToDto(existingDiscount), Times.Once);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_WhenRoomCategoryNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(default(RoomCategory));

        // Act
        var result = await _sut.DeleteAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Room category with ID '{roomCategoryId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");
        
        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenDiscountNotFound_ReturnsNotFoundResult()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();
        
        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);
        
        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(discountId, _cancellationToken))
            .ReturnsAsync(default(Discount));

        // Act
        var result = await _sut.DeleteAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.HttpStatusCode.Should().Be(404);
        result.Error.Should().Be($"Discount with ID '{discountId}' was not found.");
        result.ErrorCode.Should().Be("NOT_FOUND");
        
        _discountRepositoryMock.Verify(
            x => x.GetByIdAsync(discountId, _cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenValidRequest_DeletesAndReturns204NoContent()
    {
        // Arrange
        var hotelId = _fixture.Create<Guid>();
        var roomCategoryId = _fixture.Create<Guid>();
        var discountId = _fixture.Create<Guid>();
        
        var discount = _fixture.Build<Discount>()
            .With(d => d.Id, discountId)
            .With(d => d.RoomCategoryId, roomCategoryId)
            .Without(d => d.RoomCategory)
            .Create();
        
        var roomCategory = _fixture.Build<RoomCategory>()
            .With(rc => rc.Id, roomCategoryId)
            .With(rc => rc.HotelId, hotelId)
            .Without(rc => rc.Amenities)
            .Without(rc => rc.Discounts)
            .Without(rc => rc.Hotel)
            .Without(rc => rc.Rooms)
            .Create();
        
        _roomRepositoryMock
            .Setup(x => x.GetRoomCategoryByIdAsync(roomCategoryId, _cancellationToken))
            .ReturnsAsync(roomCategory);
        
        _discountRepositoryMock
            .Setup(x => x.GetByIdAsync(discountId, _cancellationToken))
            .ReturnsAsync(discount);

        // Act
        var result = await _sut.DeleteAsync(hotelId, roomCategoryId, discountId, _cancellationToken);

        // Assert
        result.IsSuccess.Should().BeTrue();
        
        _discountRepositoryMock.Verify(
            x => x.DeleteAsync(discount, _cancellationToken),
            Times.Once);
    }

    #endregion
}