using AutoMapper;
using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using MockQueryable;
using Moq;

namespace GameStore.Tests;
public class PlatformServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly PlatformService _platformService;

    public PlatformServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _platformService = new PlatformService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllAsyncShouldReturnAllPlatforms()
    {
        // Arrange
        var platforms = new List<Platform>
        {
            new() { Id = Guid.NewGuid(), Type = "PC" },
            new() { Id = Guid.NewGuid(), Type = "Xbox" },
            new() { Id = Guid.NewGuid(), Type = "PlayStation" },
        };

        var expectedDtos = platforms.Select(p => new PlatformResultDto
        {
            Id = p.Id,
            Type = p.Type,
        }).ToList();

        var mockPlatforms = platforms.AsQueryable().BuildMock();

        _unitOfWorkMock
            .Setup(u => u.Platforms.GetAll())
            .Returns(mockPlatforms);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<PlatformResultDto>>(platforms))
            .Returns(expectedDtos);

        // Act
        var result = await _platformService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count());
        Assert.Equal(result, expectedDtos);
        Assert.Equal(expectedDtos[0].Id, result.ElementAt(0).Id);
        Assert.Equal(expectedDtos[2].Type, result.ElementAt(2).Type);

        // Verify
        _unitOfWorkMock.Verify(u => u.Platforms.GetAll(), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<PlatformResultDto>>(platforms), Times.Once);
    }

    [Fact]
    public async Task CreateAsyncShouldCreateAndResurn()
    {
        // Arrange
        var creationDto = new PlatformCreationDto { Type = "PC" };
        var platform = new Platform { Id = Guid.NewGuid(), Type = "PC" };
        var resultDto = new PlatformResultDto { Id = platform.Id, Type = "PC" };

        _unitOfWorkMock.Setup(u => u.Platforms.AddAsync(It.IsAny<Platform>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock.Setup(m => m.Map<PlatformResultDto>(It.IsAny<Platform>()))
            .Returns(resultDto);

        // Act
        var result = await _platformService.CreateAsync(creationDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platform.Type, result.Type);
        _unitOfWorkMock.Verify(u => u.Platforms.AddAsync(It.IsAny<Platform>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mapperMock.Verify(m => m.Map<PlatformResultDto>(It.IsAny<Platform>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncShouldDeletePlatformWhenPlatformExist()
    {
        // Arrange
        var platform = new Platform { Id = Guid.NewGuid(), Type = "PC" };

        _unitOfWorkMock.Setup(u => u.Platforms.Delete(It.IsAny<Platform>()));
        _unitOfWorkMock.Setup(u => u.Platforms.FindByIdAsync(platform.Id))
                .ReturnsAsync(platform);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _platformService.DeleteAsync(platform.Id);

        // Assert
        Assert.True(result);

        _unitOfWorkMock.Verify(u => u.Platforms.Delete(It.IsAny<Platform>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Platforms.FindByIdAsync(platform.Id), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncShouldThrowWhenPlatformDoesNotExist()
    {
        // Arrange
        var platformId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Platforms.FindByIdAsync(platformId))
            .ReturnsAsync((Platform)null);

        // Act and Assert
        await Assert.ThrowsAsync<CustomException>(() =>
            _platformService.DeleteAsync(platformId));
        _unitOfWorkMock.Verify(u => u.Platforms.FindByIdAsync(platformId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsyncShouldReturnPlatformWhenPlatformExists()
    {
        // Arrange
        var platform = new Platform { Id = Guid.NewGuid(), Type = "PC" };
        var platformDto = new PlatformResultDto { Id = platform.Id, Type = "PC" };

        _unitOfWorkMock.Setup(u => u.Platforms.FindByIdAsync(platform.Id))
            .ReturnsAsync(platform);

        _mapperMock.Setup(m => m.Map<PlatformResultDto>(platform))
            .Returns(platformDto);

        // Act
        var result = await _platformService.GetByIdAsync(platform.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(platform.Id, result.Id);
        _unitOfWorkMock.Verify(u => u.Platforms.FindByIdAsync(platform.Id), Times.Once);
        _mapperMock.Verify(m => m.Map<PlatformResultDto>(platform), Times.Once);
    }

    [Fact]
    public async Task GetByGameKeyAsyncShouldReturnPlatformsWhenGameKeyExist()
    {
        // Assert
        var gameKey = "game-123";

        var platforms = new List<Platform>
        {
            new() { Id = Guid.NewGuid(), Type = "PC" },
            new() { Id = Guid.NewGuid(), Type = "PlayStation" },
        };

        var platformDtos = platforms.Select(p => new PlatformResultDto
        {
            Id = p.Id,
            Type = p.Type,
        }).ToList();

        _unitOfWorkMock.Setup(u => u.Platforms.GetByGameKeyAsync(gameKey))
            .ReturnsAsync(platforms);

        _mapperMock.Setup(m => m.Map<IEnumerable<PlatformResultDto>>(platforms))
            .Returns(platformDtos);

        // Act
        var result = await _platformService.GetByGameKeyAsync(gameKey);

        // Arrange
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("PC", result.FirstOrDefault()?.Type);
        Assert.Equal("PlayStation", result.LastOrDefault()?.Type);

        // Verifications
        _unitOfWorkMock.Verify(u => u.Platforms.GetByGameKeyAsync(gameKey), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<PlatformResultDto>>(platforms), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsyncShouldThrowCustomExceptionWhenPlatformDoesNotExist()
    {
        // Arrange
        var platformId = Guid.NewGuid();

        _unitOfWorkMock.Setup(u => u.Platforms.FindByIdAsync(platformId))
            .ReturnsAsync((Platform)null);

        // Act & Assert
        await Assert.ThrowsAsync<CustomException>(() =>
            _platformService.GetByIdAsync(platformId));
        _unitOfWorkMock.Verify(u => u.Platforms.FindByIdAsync(platformId), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncShouldUpdatePlatformWhenPlatformExists()
    {
        // Arrange
        var platform = new Platform { Id = Guid.NewGuid(), Type = "OldType" };
        var updateDto = new PlatformUpdateDto { Id = platform.Id, Type = "NewType" };

        _unitOfWorkMock.Setup(u => u.Platforms.FindByIdAsync(platform.Id))
            .ReturnsAsync(platform);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _platformService.UpdateAsync(updateDto);

        // Assert
        Assert.True(result);
        Assert.Equal("NewType", platform.Type);
        _unitOfWorkMock.Verify(u => u.Platforms.FindByIdAsync(platform.Id), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncShouldThrowWhenPlatformDoesNotExist()
    {
        // Arrange
        var updateDto = new PlatformUpdateDto { Id = Guid.NewGuid(), Type = "NewType" };

        _unitOfWorkMock.Setup(u => u.Platforms.FindByIdAsync(updateDto.Id))
            .ReturnsAsync((Platform)null);

        // Act & Assert
        await Assert.ThrowsAsync<CustomException>(() =>
            _platformService.UpdateAsync(updateDto));
        _unitOfWorkMock.Verify(u => u.Platforms.FindByIdAsync(updateDto.Id), Times.Once);
    }
}