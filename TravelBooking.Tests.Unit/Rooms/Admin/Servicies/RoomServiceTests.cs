using AutoFixture;
using FluentAssertions;
using Moq;
using TravelBooking.Application.Mappers.Interfaces;
using TravelBooking.Application.Rooms.Dtos;
using TravelBooking.Domain.Images.interfaces;
using TravelBooking.Domain.Rooms.Entities;
using TravelBooking.Domain.Rooms.Interfaces;
using Microsoft.Extensions.Logging;
using TravelBooking.Tests.Shared;
using TravelBooking.Application.Rooms.Admin.Services.Implementations;
using TravelBooking.Application.Images.Servicies.Interfaces;

namespace TravelBooking.Tests.Rooms.Admin.Handlers;

public class RoomServiceTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IRoomRepository> _repoMock;
    private readonly Mock<IRoomMapper> _mapperMock;
    private readonly Mock<IGalleryImageRepository> _galleryImageRepoMock;
    private readonly Mock<IImageAppService> _imageAppServiceMock;
    private readonly RoomService _service;
    private readonly Mock<ILogger<RoomService>> _loggerMock;

    public RoomServiceTests()
    {
        _fixture = new Fixture().Customize(new EntityCustomization());
        _repoMock = new Mock<IRoomRepository>();
        _mapperMock = new Mock<IRoomMapper>();
        _galleryImageRepoMock = new Mock<IGalleryImageRepository>();
        _imageAppServiceMock = new Mock<IImageAppService>();
        _loggerMock = new Mock<ILogger<RoomService>>();

        _service = new RoomService(
            _repoMock.Object,
            _mapperMock.Object,
            _galleryImageRepoMock.Object,
            _imageAppServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task GetRoomsAsync_ShouldReturnPagedMappedResults_WhenRepositoryHasManyRooms()
    {
        // Arrange
        var allRooms = _fixture
        .CreateMany<Room>(50)
        .ToList();


        _repoMock.Setup(r => r.GetRoomsAsync(It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(allRooms);

        // mapper maps each Room to a RoomDto
        _mapperMock.Setup(m => m.Map(It.IsAny<Room>()))
            .Returns<Room>(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                AdultsCapacity = 2,
                ChildrenCapacity = 1,
                CategoryName = "Category"
            });

        // Act - request page 2, pageSize 10
        var result = await _service.GetRoomsAsync(null, page: 2, pageSize: 10, CancellationToken.None);

        // Assert
        result.Should().HaveCount(10);
        result.Select(r => r.CategoryName).Should().AllBeEquivalentTo("Category");
        _repoMock.Verify(r => r.GetRoomsAsync(null, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map(It.IsAny<Room>()), Times.Exactly(10));
    }

    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnDto_WhenFound()
    {
        // Arrange
        var room = _fixture.Create<Room>();
        var dto = new RoomDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            AdultsCapacity = 2,
            ChildrenCapacity = 1,
            CategoryName = "Cat"
        };
        _repoMock.Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(room);
        _mapperMock.Setup(m => m.Map(room)).Returns(dto);

        // Act
        var result = await _service.GetRoomByIdAsync(room.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(room.Id);
        _repoMock.Verify(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetRoomByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        var result = await _service.GetRoomByIdAsync(id, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateRoomAsync_ShouldReturnNewGuid_AndCallRepoAdd()
    {
        // Arrange
        var dto = _fixture.Create<CreateRoomDto>();
        var mappedRoom = new Room { RoomNumber = dto.RoomNumber, RoomCategoryId = dto.RoomCategoryId };
        _mapperMock.Setup(m => m.Map(dto)).Returns(mappedRoom);
        _repoMock.Setup(r => r.AddAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask)
            .Verifiable();

        // Act
        var room = await _service.CreateRoomAsync(dto, CancellationToken.None);

        // Assert
        room.Should().NotBeNull();
        room.Id.Should().NotBe(Guid.Empty);
        _repoMock.Verify(r => r.AddAsync(It.Is<Room>(x => x.Id == room.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldThrow_WhenRoomNotFound()
    {
        // Arrange
        var dto = _fixture.Create<UpdateRoomDto>();
        _repoMock.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Room?)null);

        // Act
        Func<Task> act = async () => await _service.UpdateRoomAsync(dto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Room with ID {dto.Id} not found.");
    }

    [Fact]
    public async Task UpdateRoomAsync_ShouldUpdateAndCallRepo_WhenRoomExists()
    {
        // Arrange
        var dto = _fixture.Create<UpdateRoomDto>();
        var existing = new Room { Id = dto.Id, RoomNumber = "old", RoomCategoryId = Guid.NewGuid()};
        _repoMock.Setup(r => r.GetByIdAsync(dto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _mapperMock.Setup(m => m.UpdateRoomFromDto(dto, existing)).Verifiable();
        _repoMock.Setup(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _service.UpdateRoomAsync(dto, CancellationToken.None);

        // Assert
        _mapperMock.Verify(m => m.UpdateRoomFromDto(dto, existing), Times.Once);
        _repoMock.Verify(r => r.UpdateAsync(existing, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteRoomAsync_ShouldNotThrow_WhenRoomDoesNotExist()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repoMock.Setup(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync((Room?)null);

        // Act
        Func<Task> act = async () => await _service.DeleteRoomAsync(id, CancellationToken.None);

        // Assert
        await act.Should().NotThrowAsync();
        _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Room>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteRoomAsync_ShouldCallRepoDelete_WhenRoomExists()
    {
        // Arrange
        var room = _fixture.Build<Room>()
        .Without(r => r.Gallery)
        .Without(r => r.Bookings)
        .Without(r => r.RoomCategory)
        .Create();

        _repoMock.Setup(r => r.GetByIdAsync(room.Id, It.IsAny<CancellationToken>())).ReturnsAsync(room);
        _repoMock.Setup(r => r.DeleteAsync(room, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask).Verifiable();

        // Act
        await _service.DeleteRoomAsync(room.Id, CancellationToken.None);

        // Assert
        _repoMock.Verify(r => r.DeleteAsync(room, It.IsAny<CancellationToken>()), Times.Once);
    }
}