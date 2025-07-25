using AutoMapper;
using GameStore.Application.DTOs.Genres;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GameStore.Tests;
public class GenreServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GenreService>> _loggerMock;
    private readonly GenreService _genreService;

    public GenreServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GenreService>>();
        _genreService = new GenreService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AddAsyncValidInputShouldAddGenreAndReturnResult()
    {
        // Arrange
        var genreCreationDto = new GenreCreationDto
        {
            Name = "Action",
            ParentGenreId = null,
        };

        var createdGenre = new Genre
        {
            Id = Guid.NewGuid(),
            Name = genreCreationDto.Name,
            ParentGenreId = null,
        };

        _unitOfWorkMock
            .Setup(u => u.Genres.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Genre)null);

        _unitOfWorkMock
            .Setup(u => u.Genres.AddAsync(It.IsAny<Genre>()))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<GenreResultDto>(It.IsAny<Genre>()))
            .Returns(new GenreResultDto { Id = createdGenre.Id, Name = createdGenre.Name });

        // Act
        var result = await _genreService.AddAsync(genreCreationDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(genreCreationDto.Name, result.Name);

        _unitOfWorkMock.Verify(u => u.Genres.AddAsync(It.IsAny<Genre>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsyncInvalidParentGenreShouldThrowException()
    {
        // Arrange
        var genreCreationDto = new GenreCreationDto
        {
            Name = "Adventure",
            ParentGenreId = Guid.NewGuid(),
        };

        _unitOfWorkMock
            .Setup(u => u.Genres.FindByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _genreService.AddAsync(genreCreationDto));
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("Parent genre not found", exception.Message);

        _unitOfWorkMock.Verify(u => u.Genres.AddAsync(It.IsAny<Genre>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task DeleteAsyncValidGenreShouldDeleteGenreAndReturnTrue()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var existingGenre = new Genre { Id = genreId };

        _unitOfWorkMock
            .Setup(u => u.Genres.FindByIdAsync(genreId))
            .ReturnsAsync(existingGenre);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _genreService.DeleteAsync(genreId);

        // Assert
        Assert.True(result);

        _unitOfWorkMock.Verify(u => u.Genres.Delete(existingGenre), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncInvalidGenreShouldThrowException()
    {
        // Arrange
        var genreId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(u => u.Genres.FindByIdAsync(genreId))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _genreService.DeleteAsync(genreId));
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal($"Genre with ID {genreId} does not exist.", exception.Message);

        _unitOfWorkMock.Verify(u => u.Genres.Delete(It.IsAny<Genre>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetAllAsyncShouldReturnAllGenres()
    {
        // Arrange
        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Action" },
            new() { Id = Guid.NewGuid(), Name = "Adventure" },
        };
        var expectedDtos = genres.Select(g => new GenreResultDto
        {
            Id = g.Id,
            Name = g.Name,
        });

        var mockGenres = genres.AsQueryable().BuildMock();

        _unitOfWorkMock
            .Setup(u => u.Genres.GetAll())
            .Returns(mockGenres);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<GenreResultDto>>(genres))
            .Returns(expectedDtos);

        // Act
        var result = await _genreService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        _unitOfWorkMock.Verify(u => u.Genres.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsyncValidGenreShouldReturnGenre()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var existingGenre = new Genre { Id = genreId, Name = "Action" };

        _unitOfWorkMock
            .Setup(u => u.Genres.FindByIdAsync(genreId))
            .ReturnsAsync(existingGenre);

        _mapperMock
            .Setup(m => m.Map<GenreResultDto>(existingGenre))
            .Returns(new GenreResultDto { Id = genreId, Name = existingGenre.Name });

        // Act
        var result = await _genreService.GetByIdAsync(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(existingGenre.Name, result.Name);

        _unitOfWorkMock.Verify(u => u.Genres.FindByIdAsync(genreId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsyncInvalidGenreShouldThrowException()
    {
        // Arrange
        var genreId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(u => u.Genres.FindByIdAsync(genreId))
            .ReturnsAsync((Genre)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _genreService.GetByIdAsync(genreId));
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal($"Genre with ID {genreId} does not exist.", exception.Message);

        _unitOfWorkMock.Verify(u => u.Genres.FindByIdAsync(genreId), Times.Once);
    }

    [Fact]
    public async Task GetByGameKeyAsyncShouldReturnGenresWhenGameKeyExist()
    {
        // Assert
        var gameKey = "game-123";

        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "Action" },
            new() { Id = Guid.NewGuid(), Name = "Mini" },
        };

        var genreDtos = genres.Select(p => new GenreResultDto
        {
            Id = p.Id,
            Name = p.Name,
        }).ToList();

        _unitOfWorkMock.Setup(u => u.Genres.GetByGameKeyAsync(gameKey))
            .ReturnsAsync(genres);

        _mapperMock.Setup(m => m.Map<IEnumerable<GenreResultDto>>(genres))
            .Returns(genreDtos);

        // Act
        var result = await _genreService.GetByGameKeyAsync(gameKey);

        // Arrange
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Equal("Action", result.FirstOrDefault()?.Name);
        Assert.Equal("Mini", result.LastOrDefault()?.Name);

        // Verifications
        _unitOfWorkMock.Verify(u => u.Genres.GetByGameKeyAsync(gameKey), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<GenreResultDto>>(genres), Times.Once);
    }

    [Fact]
    public async Task GetByGameKeyAsyncInvalidGameKeyShouldReturnEmptyList()
    {
        // Arrange
        var gameKey = "invalidGameKey";

        _unitOfWorkMock
            .Setup(u => u.Genres.GetByGameKeyAsync(gameKey))
            .ReturnsAsync(new List<Genre>());

        _mapperMock
            .Setup(m => m.Map<IEnumerable<GenreResultDto>>(It.IsAny<IEnumerable<Genre>>()))
            .Returns(new List<GenreResultDto>());

        // Act
        var result = await _genreService.GetByGameKeyAsync(gameKey);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _unitOfWorkMock.Verify(u => u.Genres.GetByGameKeyAsync(gameKey), Times.Once);
    }

    [Fact]
    public async Task GetByParentIdAsyncValidParentIdShouldReturnGenres()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var genres = new List<Genre>
        {
            new() { Id = Guid.NewGuid(), Name = "SubGenre1", ParentGenreId = parentId },
            new() { Id = Guid.NewGuid(), Name = "SubGenre2", ParentGenreId = parentId },
        };

        _unitOfWorkMock
            .Setup(u => u.Genres.GetByParentIdAsync(parentId))
            .ReturnsAsync(genres);

        _mapperMock
            .Setup(m => m.Map<IEnumerable<GenreResultDto>>(genres))
            .Returns(new List<GenreResultDto>
            {
            new() { Id = genres[0].Id, Name = genres[0].Name },
            new() { Id = genres[1].Id, Name = genres[1].Name },
            });

        // Act
        var result = await _genreService.GetByParentIdAsync(parentId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "SubGenre1");
        Assert.Contains(result, r => r.Name == "SubGenre2");

        _unitOfWorkMock.Verify(u => u.Genres.GetByParentIdAsync(parentId), Times.Once);
    }

    [Fact]
    public async Task GetByParentIdAsyncInvalidParentIdShouldReturnEmptyList()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(u => u.Genres.GetByParentIdAsync(parentId))
            .ReturnsAsync(new List<Genre>()); // No genres found.

        _mapperMock
            .Setup(m => m.Map<IEnumerable<GenreResultDto>>(It.IsAny<IEnumerable<Genre>>()))
            .Returns(new List<GenreResultDto>()); // Empty result mapping.

        // Act
        var result = await _genreService.GetByParentIdAsync(parentId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _unitOfWorkMock.Verify(u => u.Genres.GetByParentIdAsync(parentId), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncShouldUpdateGenreWhenGenreExist()
    {
        // Arrange
        var genre = new Genre()
        {
            Id = Guid.NewGuid(),
            Name = "Test",
        };
        var updateDto = new GenreUpdateDto() { Id = genre.Id, Name = genre.Name };

        _unitOfWorkMock.Setup(u => u.Genres.FindByIdAsync(genre.Id))
            .ReturnsAsync(genre);

        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _genreService.UpdateAsync(updateDto);

        // Assert
        Assert.True(result);
        Assert.Equal("Test", genre.Name);

        _unitOfWorkMock.Verify(u => u.Genres.FindByIdAsync(genre.Id), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsyncShouldThrowWhenGenreDoesNotExist()
    {
        // Arrange
        var updateDto = new GenreUpdateDto { Id = Guid.NewGuid(), Name = "Test" };

        _unitOfWorkMock.Setup(u => u.Genres.FindByIdAsync(updateDto.Id))
            .ReturnsAsync((Genre)null);

        // Act and Assert
        await Assert.ThrowsAsync<CustomException>(() =>
            _genreService.UpdateAsync(updateDto));
        _unitOfWorkMock.Verify(u => u.Genres.FindByIdAsync(updateDto.Id), Times.Once);
    }
}
