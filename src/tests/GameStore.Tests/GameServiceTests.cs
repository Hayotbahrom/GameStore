using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.Exceptions;
using GameStore.Application.Services;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;

namespace GameStore.Tests;
public class GameServiceTests
{
    private static readonly JsonSerializerOptions? _testJsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<GameService>> _loggerMock;
    private readonly GameService _gameService;

    public GameServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<GameService>>();
        _gameService = new GameService(
            _unitOfWorkMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsyncShouldReturnAllGames()
    {
        // Arrange
        var games = new List<Game>()
        {
            new() { Id = Guid.NewGuid(), Name = "Test game", Key = "test-key" },
            new() { Id = Guid.NewGuid(), Name = "Generals 007", Key = "07" },
        };

        var expectedDtos = games.Select(x => new GameResultDto
        {
            Id = x.Id,
            Name = x.Name!,
            Key = x.Key!,
        });

        var mockGames = games.AsQueryable().BuildMock();

        _unitOfWorkMock.Setup(u => u.Games.GetAll())
            .Returns(mockGames);

        _mapperMock.Setup(m => m.Map<IEnumerable<GameResultDto>>(It.IsAny<List<Game>>()))
            .Returns(expectedDtos);

        // Act
        var result = await _gameService.GetAllAsync();

        // Asset
        Assert.Equal(2, result.Count());
        Assert.NotNull(result);

        _unitOfWorkMock.Verify(u => u.Games.GetAll(), Times.Once);
    }

    [Fact]
    public async Task GetGamesByGenreIdAsyncValidGenreIdReturnsMatchingGames()
    {
        var genreId = Guid.NewGuid();
        var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Game",
                Key = "1.6",
                GameGenres = new List<GameGenre> { new() { GenreId = genreId } },
                GamePlatforms = new List<GamePlatform>(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Game 2",
                Key = "20",
                GameGenres = new List<GameGenre> { new() { GenreId = Guid.NewGuid() } },
                GamePlatforms = new List<GamePlatform>(),
            },
        };
        var expectedDtos = games.Where(x => x.GameGenres.Any(gg => gg.GenreId == genreId))
            .Select(g => new GameResultDto
            {
                Id = g.Id,
                Name = g.Name!,
                Key = g.Key!,
            });

        _unitOfWorkMock.Setup(u => u.Games.GetGamesByGenreIdAsync(genreId))
            .ReturnsAsync(games);

        _mapperMock.Setup(m => m.Map<IEnumerable<GameResultDto>>(games))
            .Returns(expectedDtos);

        // Act
        var result = await _gameService.GetGamesByGenreIdAsync(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);

        _unitOfWorkMock.Verify(u => u.Games.GetGamesByGenreIdAsync(genreId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<GameResultDto>>(games), Times.Once);
    }

    [Fact]
    public async Task GetGamesByGenreIdAsyncReturnsEmptyListWhenNoGamesExist()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var games = new List<Game>();

        _unitOfWorkMock.Setup(u => u.Games.GetGamesByGenreIdAsync(genreId))
            .ReturnsAsync(games);

        _mapperMock.Setup(m => m.Map<IEnumerable<GameResultDto>>(games))
            .Returns(new List<GameResultDto>());

        // Act
        var result = await _gameService.GetGamesByGenreIdAsync(genreId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _unitOfWorkMock.Verify(u => u.Games.GetGamesByGenreIdAsync(genreId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<GameResultDto>>(games), Times.Once);
    }

    [Fact]
    public async Task GetGamesByPlatformIdAsyncShouldResturnGamesListWhenGivenPlatformIdExist()
    {
        var platformId = Guid.NewGuid();
        var games = new List<Game>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Game",
                Key = "1.6",
                GamePlatforms = new List<GamePlatform> { new() { PlatformId = platformId } },
                GameGenres = new List<GameGenre>(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Name = "Test Game 2",
                Key = "20",
                GamePlatforms = new List<GamePlatform> { new() { PlatformId = platformId } },
                GameGenres = new List<GameGenre>(),
            },
        };
        var expectedDtos = games.Where(x => x.GamePlatforms.Any(gg => gg.PlatformId == platformId))
            .Select(g => new GameResultDto
            {
                Id = g.Id,
                Name = g.Name!,
                Key = g.Key!,
            });

        _unitOfWorkMock.Setup(u => u.Games.GetGamesByPlatformIdAsync(platformId))
            .ReturnsAsync(games);

        _mapperMock.Setup(m => m.Map<IEnumerable<GameResultDto>>(games))
            .Returns(expectedDtos);

        // Act
        var result = await _gameService.GetGamesByPlatformIdAsync(platformId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());

        _unitOfWorkMock.Verify(u => u.Games.GetGamesByPlatformIdAsync(platformId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<GameResultDto>>(games), Times.Once);
    }

    [Fact]
    public async Task GetGamesByPlatformIdAsyncReturnsEmptyListWithGivenPlatformIdWhenNoGamesExist()
    {
        // Arrange
        var platformId = Guid.NewGuid();
        var games = new List<Game>();

        _unitOfWorkMock.Setup(u => u.Games.GetGamesByPlatformIdAsync(platformId))
            .ReturnsAsync(games);

        _mapperMock.Setup(m => m.Map<IEnumerable<GameResultDto>>(games))
            .Returns(new List<GameResultDto>());

        // Act
        var result = await _gameService.GetGamesByPlatformIdAsync(platformId);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);

        _unitOfWorkMock.Verify(u => u.Games.GetGamesByPlatformIdAsync(platformId), Times.Once);
        _mapperMock.Verify(m => m.Map<IEnumerable<GameResultDto>>(games), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsyncValidIdShouldReturnGameResultDto()
    {
        // Arrange
        var gameId = Guid.NewGuid();
        var game = new Game
        {
            Id = gameId,
            Name = "Call of Duty",
            Key = "call-of-duty",
            Description = "A first-person shooter game.",
        };

        var gameResultDto = new GameResultDto
        {
            Id = gameId,
            Name = "Call of Duty",
            Key = "call-of-duty",
            Description = "A first-person shooter game.",
        };

        _unitOfWorkMock
            .Setup(u => u.Games.GetFullGameByIdAsync(gameId))
            .ReturnsAsync(game);

        _mapperMock
            .Setup(m => m.Map<GameResultDto>(game))
            .Returns(gameResultDto);

        // Act
        var result = await _gameService.GetByIdAsync(gameId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameId, result.Id);
        Assert.Equal("Call of Duty", result.Name);

        _unitOfWorkMock.Verify(u => u.Games.GetFullGameByIdAsync(gameId), Times.Once);
        _mapperMock.Verify(m => m.Map<GameResultDto>(game), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsyncInvalidIdShouldThrowCustomException()
    {
        // Arrange
        var gameId = Guid.NewGuid();

        _unitOfWorkMock
            .Setup(u => u.Games.GetFullGameByIdAsync(gameId))
            .ReturnsAsync((Game)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.GetByIdAsync(gameId));
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("Game is not found.", exception.Message);

        _unitOfWorkMock.Verify(u => u.Games.GetFullGameByIdAsync(gameId), Times.Once);
        _mapperMock.Verify(m => m.Map<GameResultDto>(It.IsAny<Game>()), Times.Never);
    }

    [Fact]
    public async Task GetByKeyAsyncValidKeyShouldReturnGameResultDto()
    {
        // Arrange
        var gameKey = "minecraft";
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Minecraft",
            Key = gameKey,
            Description = "A sandbox game.",
        };

        var gameResultDto = new GameResultDto
        {
            Id = game.Id,
            Name = "Minecraft",
            Key = gameKey,
            Description = "A sandbox game.",
        };

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameKey))
            .ReturnsAsync(game);

        _mapperMock
            .Setup(m => m.Map<GameResultDto>(game))
            .Returns(gameResultDto);

        // Act
        var result = await _gameService.GetByKeyAsync(gameKey);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameKey, result.Key);
        Assert.Equal("Minecraft", result.Name);

        _unitOfWorkMock.Verify(u => u.Games.GetByKeyAsync(gameKey), Times.Once);
        _mapperMock.Verify(m => m.Map<GameResultDto>(game), Times.Once);
    }

    [Fact]
    public async Task GetByKeyAsyncInvalidKeyShouldThrowCustomException()
    {
        // Arrange
        var gameKey = "invalid-key";

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameKey))
            .ReturnsAsync((Game)null);

        _mapperMock
            .Setup(m => m.Map<GameResultDto>(null))
            .Returns((GameResultDto)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.GetByKeyAsync(gameKey));
        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("Game is not found", exception.Message);

        _unitOfWorkMock.Verify(u => u.Games.GetByKeyAsync(gameKey), Times.Once);
        _mapperMock.Verify(m => m.Map<GameResultDto>(It.IsAny<Game>()), Times.Once);
    }

    [Fact]
    public async Task AddAsyncValidGameShouldAddGameAndReturnResult()
    {
        // Arrange
        var gameCreationDto = new GameCreationDto
        {
            Name = "Counter Strike",
            Description = "The most famous game in the world.",
            GenreIds = new List<Guid> { Guid.NewGuid() },
            PlatformIds = new List<Guid> { Guid.NewGuid() },
        };
        var gameEntity = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Counter Strike",
            Description = "The most famous game in the world.",
            Key = "counter-strike",
        };

        _unitOfWorkMock
            .Setup(u => u.Genres.GetExistingIdsAsync(gameCreationDto.GenreIds))
            .ReturnsAsync(gameCreationDto.GenreIds);

        _unitOfWorkMock
            .Setup(u => u.Platforms.GetExistingIdsAsync(gameCreationDto.PlatformIds))
            .ReturnsAsync(gameCreationDto.PlatformIds);

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameEntity.Key))
            .ReturnsAsync((Game)null);

        _unitOfWorkMock
            .Setup(u => u.Games.AddAsync(gameEntity))
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<GameResultDto>(It.IsAny<Game>()))
            .Returns(new GameResultDto { Id = gameEntity.Id, Name = gameEntity.Name });

        // Act
        var result = await _gameService.AddAsync(gameCreationDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(gameEntity.Name, result.Name);

        _unitOfWorkMock.Verify(u => u.Games.AddAsync(It.Is<Game>(g => g.Name == gameCreationDto.Name)), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task AddAsyncExistingGameKeyShouldThrowCustomException()
    {
        // Arrange
        var gameCreationDto = new GameCreationDto
        {
            Name = "Minecraft",
            Key = "minecraft",
            GenreIds = new List<Guid> { Guid.NewGuid() },
            PlatformIds = new List<Guid> { Guid.NewGuid() },
        };

        var existingGame = new Game
        {
            Id = Guid.NewGuid(),
            Name = "Minecraft",
            Key = "minecraft",
            GameGenres = new List<GameGenre>
            {
                new() { GenreId = Guid.NewGuid() },
            },
            GamePlatforms = new List<GamePlatform>
            {
                new() { PlatformId = Guid.NewGuid() },
            },
        };

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameCreationDto.Key))
            .ReturnsAsync(existingGame);

        _unitOfWorkMock
            .Setup(u => u.Genres.GetExistingIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync((List<Guid> ids) => ids);

        _unitOfWorkMock
            .Setup(u => u.Platforms.GetExistingIdsAsync(It.IsAny<List<Guid>>()))
            .ReturnsAsync((List<Guid> ids) => ids);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(409, exception.StatusCode);
        Assert.Equal($"Game key '{gameCreationDto.Key}' already exists.", exception.Message);

        _unitOfWorkMock.Verify(u => u.Games.AddAsync(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task AddAsyncInvalidGenreIdsInDataBaseShouldThrowCustomException()
    {
        // Arrange
        var genreId1 = Guid.NewGuid();
        var genreId2 = Guid.NewGuid();

        var gameCreationDto = new GameCreationDto
        {
            Name = "Invalid Genre test",
            GenreIds = new List<Guid> { genreId1, genreId2 },
            PlatformIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
        };

        _unitOfWorkMock
            .Setup(u => u.Genres.GetExistingIdsAsync(gameCreationDto.GenreIds))
            .ReturnsAsync(new List<Guid> { genreId1 });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(400, ex.StatusCode);
        Assert.Equal($"The following genre IDs are invalid: {genreId2}", ex.Message);
    }

    [Fact]
    public async Task AddAsyncInvalidPlatformIdsInDataBaseShouldThrowCustomException()
    {
        var platformId1 = Guid.NewGuid();
        var platformId2 = Guid.NewGuid();

        var gameCreationDto = new GameCreationDto
        {
            Name = "Invalid Platform Test",
            GenreIds = new List<Guid> { Guid.NewGuid() },
            PlatformIds = new List<Guid> { platformId1, platformId2 },
        };

        _unitOfWorkMock
            .Setup(u => u.Genres.GetExistingIdsAsync(gameCreationDto.GenreIds))
            .ReturnsAsync(gameCreationDto.GenreIds);

        _unitOfWorkMock
            .Setup(u => u.Platforms.GetExistingIdsAsync(gameCreationDto.PlatformIds))
            .ReturnsAsync(new List<Guid> { platformId1 });

        // Act & Assert
        var ex = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(400, ex.StatusCode);
        Assert.Equal($"The following platform IDs are invalid: {platformId2}", ex.Message);
    }

    [Fact]
    public async Task AddAsyncEmptyGenreIdsShouldThrowCustomException()
    {
        var gameCreationDto = new GameCreationDto
        {
            Name = "Test Game",
            Key = "gamekey",
            GenreIds = new List<Guid>(),
            PlatformIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
        };

        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal("At least one genre must be provided.", exception.Message);
    }

    [Fact]
    public async Task AddAsyncNullGenreIdsShouldThrowCustomException()
    {
        var gameCreationDto = new GameCreationDto
        {
            Name = "Test Game",
            Key = "gamekey",
            GenreIds = null,
            PlatformIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
        };

        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal("At least one genre must be provided.", exception.Message);
    }

    [Fact]
    public async Task AddAsyncEmptyPlatformIdsShouldThrowCustomException()
    {
        var gameCreationDto = new GameCreationDto
        {
            Name = "Test Game",
            Key = "gamekey",
            PlatformIds = new List<Guid>(),
            GenreIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
        };

        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal("At least one platform must be provided.", exception.Message);
    }

    [Fact]
    public async Task AddAsyncNullPlatformIdsShouldThrowCustomException()
    {
        var gameCreationDto = new GameCreationDto
        {
            Name = "Test Game",
            Key = "gamekey",
            PlatformIds = null,
            GenreIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
        };

        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(gameCreationDto));
        Assert.Equal(400, exception.StatusCode);
        Assert.Equal("At least one platform must be provided.", exception.Message);
    }

    [Fact]
    public async Task AddAsyncNullGameShouldThrowCustomException()
    {
        // Act and Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.AddAsync(null));

        Assert.Equal(400, exception.StatusCode);
        Assert.Equal("Game cannot be null.", exception.Message);
    }

    [Fact]
    public async Task DeleteAsyncValidKeyShouldDeleteGameAndReturnTrue()
    {
        // Arrange
        var gameKey = "minecraft";
        var existingGame = new Game { Id = Guid.NewGuid(), Key = gameKey };

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameKey))
            .ReturnsAsync(existingGame);

        _unitOfWorkMock
            .Setup(u => u.Games.Delete(existingGame))
            .Verifiable();

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _gameService.DeleteAsync(gameKey);

        // Assert
        Assert.True(result);

        _unitOfWorkMock.Verify(u => u.Games.GetByKeyAsync(gameKey), Times.Once);
        _unitOfWorkMock.Verify(u => u.Games.Delete(existingGame), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsyncInvalidKeyShouldThrowCustomException()
    {
        // Arrange
        var gameKey = "invalid-key";

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameKey))
            .ReturnsAsync((Game)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.DeleteAsync(gameKey));

        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("Game not found with given Key.", exception.Message);

        _unitOfWorkMock.Verify(u => u.Games.Delete(It.IsAny<Game>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GenerateGameFileAsyncValidKeyShouldReturnFileContent()
    {
        // Arrange
        var gameKey = "minecraft";
        var existingGame = new Game { Id = Guid.NewGuid(), Name = "Minecraft", Key = gameKey };
        var timestamp = DateTime.UtcNow.ToString("ddMMyyyy-HHmmss");
        var expectedFileName = $"{gameKey}_{timestamp}.txt";

        var gameDto = new GameResultDto { Id = existingGame.Id, Name = existingGame.Name, Key = gameKey };

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(gameKey))
            .ReturnsAsync(existingGame);

        _mapperMock
            .Setup(m => m.Map<GameResultDto>(existingGame))
            .Returns(gameDto);

        // Act
        var (fileBytes, fileName) = await _gameService.GenerateGameFileAsync(gameKey);

        // Assert
        Assert.NotNull(fileBytes);
        Assert.NotNull(fileName);

        var serializedGame = JsonSerializer.Serialize(gameDto, _testJsonOptions);
        var expectedContent = Encoding.UTF8.GetBytes(serializedGame);

        Assert.Equal(expectedContent, fileBytes);
        Assert.Equal(expectedFileName, fileName);

        _unitOfWorkMock.Verify(u => u.Games.GetByKeyAsync(gameKey), Times.Once);
        _mapperMock.Verify(m => m.Map<GameResultDto>(existingGame), Times.Once);
    }

    [Fact]
    public async Task GenerateGameFileAsyncInvalidKeyShouldThrowException()
    {
        var invalidKey = "invalid-key";

        _unitOfWorkMock
            .Setup(u => u.Games.GetByKeyAsync(invalidKey))
            .ReturnsAsync(null as Game);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.GenerateGameFileAsync(invalidKey));

        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("Game key is not found.", exception.Message);
    }

    [Fact]
    public async Task GetTotalGamesCountAsyncShouldReturnCount()
    {
        // Arrange
        var games = new List<Game>
        {
            new(),
            new(),
            new(),
        };

        var mockGames = games.AsQueryable().BuildMock();

        _unitOfWorkMock
            .Setup(u => u.Games.GetAll())
            .Returns(mockGames);

        // Act
        var result = await _gameService.GetTotalGamesCountAsync();

        // Assert
        Assert.Equal(3, result);
    }

    [Fact]
    public async Task UpdateAsyncExistGameKeyShouldThrowCustomException()
    {
        var gameUpdateDto = new GameUpdateDto
        {
            Id = Guid.NewGuid(),
            Name = "Test",
            Key = "Test",
            Description = "Test",
            GenreIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() },
            PlatformIds = new List<Guid> { Guid.NewGuid() },
        };

        var game = new Game
        {
            Id = gameUpdateDto.Id,
            Name = gameUpdateDto.Name,
            Key = gameUpdateDto.Key,
            Description = gameUpdateDto.Description,
        };

        _unitOfWorkMock
            .Setup(u => u.Games.Update(game));

        _unitOfWorkMock
            .Setup(u => u.Games.GetFullGameByIdAsync(gameUpdateDto.Id))
            .ReturnsAsync(game);

        _unitOfWorkMock
            .Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _gameService.UpdateAsync(gameUpdateDto);

        // Assert
        Assert.True(result);
        Assert.Equal(game.Key, gameUpdateDto.Key);
    }

    [Fact]
    public async Task UpdateAsyncInvalidGameIdShouldThrowCustomException()
    {
        var game = new GameUpdateDto { Id = Guid.NewGuid(), Name = "Test", Key = "Test", };

        _unitOfWorkMock
            .Setup(u => u.Games.GetFullGameByIdAsync(game.Id))
            .ReturnsAsync((Game)null);

        var exception = await Assert.ThrowsAsync<CustomException>(() => _gameService.UpdateAsync(game));

        Assert.Equal(404, exception.StatusCode);
        Assert.Equal("Game is not found.", exception.Message);

        _unitOfWorkMock.Verify(u => u.Games.FindByIdAsync(game.Id), Times.Never());
        _unitOfWorkMock.Verify(u => u.Games.GetFullGameByIdAsync(game.Id), Times.Once());
    }
}
