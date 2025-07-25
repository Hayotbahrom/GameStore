using System.Text;
using GameStore.API.Controllers;
using GameStore.Application.DTOs.Games;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests;

public class GameControllerTests
{
    private readonly Mock<IGameService> _mockGameService;
    private readonly GamesController _controller;

    public GameControllerTests()
    {
        _mockGameService = new Mock<IGameService>();
        _controller = new GamesController(_mockGameService.Object);
    }

    [Fact]
    public async Task GetAllAsyncReturnsOkResultWithListOfGames()
    {
        // Arrange
        var games = new List<GameResultDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Game 1", Key = "game-1" },
            new() { Id = Guid.NewGuid(), Name = "Game 2", Key = "game-2" },
        };

        _mockGameService.Setup(s => s.GetAllAsync()).ReturnsAsync(games);

        // Act
        var result = await _controller.GetAllAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<GameResultDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetGamesByGenreIdAsyncReturnsOkResult()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var games = new List<GameResultDto> { new() { Id = Guid.NewGuid(), Name = "Game 1" } };
        _mockGameService.Setup(s => s.GetGamesByGenreIdAsync(genreId)).ReturnsAsync(games);

        // Act
        var result = await _controller.GetGamesByGenreIdAsync(genreId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<GameResultDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetGamesByPlatformIdAsyncReturnsOkResultWithListOfGames()
    {
        var platformId = Guid.NewGuid();
        var games = new List<GameResultDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Game 1", Key = "game-1" },
            new() { Id = Guid.NewGuid(), Name = "Game 2", Key = "game-2" },
        };

        _mockGameService.Setup(s => s.GetGamesByPlatformIdAsync(platformId)).ReturnsAsync(games);

        var result = await _controller.GetGamesByPlatformIdAsync(platformId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<GameResultDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task DeleteAsyncReturnsOkResultWithBool()
    {
        // Arrange
        var key = "game-1";
        _mockGameService.Setup(s => s.DeleteAsync(key)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAsync(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var success = Assert.IsType<bool>(okResult.Value);
        Assert.True(success);
    }

    [Fact]
    public async Task CreateAsyncReturnsOkResultWithCreatedGame()
    {
        // Arrange
        var dto = new GameCreationDto
        {
            Name = "new Game",
            Key = "coolGame",
            Description = "Description",
        };

        var expectedResult = new GameResultDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Key = dto.Key,
            Description = dto.Description,
        };

        _mockGameService
            .Setup(s => s.AddAsync(dto))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GameResultDto>(okResult.Value);

        Assert.Equal(expectedResult.Id, returnValue.Id);
        Assert.Equal(dto.Name, returnValue.Name);
        Assert.Equal(dto.Key, returnValue.Key);
    }

    [Fact]
    public async Task GetByKeyAsyncReturnsOkResultWithGame()
    {
        // Arrange
        var key = "gamekey";
        var expected = new GameResultDto
        {
            Id = Guid.NewGuid(),
            Name = "Game",
            Key = key,
        };

        _mockGameService.Setup(s => s.GetByKeyAsync(key)).ReturnsAsync(expected);

        // act
        var result = await _controller.GetByKeyAsync(key);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GameResultDto>(okResult?.Value);
        Assert.Equal(key, returnValue.Key);
        Assert.Equal(expected.Id.ToString(), returnValue.Id.ToString());
    }

    [Fact]
    public async Task GetByIdAsyncReturnsOkResultWithGame()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new GameResultDto { Id = id, Name = "Game 1", Key = "game-1" };

        _mockGameService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(expected);

        // Act
        var result = await _controller.GetByIdAsync(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GameResultDto>(okResult.Value);
        Assert.Equal(id, returnValue.Id);
    }

    [Fact]
    public async Task UpdateAsyncReturnsOkResultWithUpdatedGame()
    {
        // Arrange
        var dto = new GameUpdateDto { Id = Guid.NewGuid(), Name = "Updated Game", Key = "game-1" };

        _mockGameService.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateAsync(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<bool>(okResult.Value);
        Assert.True(returnValue);
    }

    [Fact]
    public async Task DownloadGameFileReturnsFile()
    {
        // Arrange
        var key = "test-game";
        var fileBytes = Encoding.UTF8.GetBytes("test content");

        _mockGameService
            .Setup(s => s.GenerateGameFileAsync(key))
            .ReturnsAsync(fileBytes);

        var controller = new GamesController(_mockGameService.Object);

        // Act
        var result = await controller.DownloadGameFile(key);

        // Assert
        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("text/plain", fileResult.ContentType);
        Assert.Equal(fileBytes, fileResult.FileContents);
    }

    [Fact]
    public async Task DownloadGameFileReturnsNotFoundWhenExceptionThrown()
    {
        // Arrange
        var key = "invalid-game";

        _mockGameService
            .Setup(s => s.GenerateGameFileAsync(key))
            .ThrowsAsync(new Exception("Game not found"));

        var controller = new GamesController(_mockGameService.Object);

        // Act
        var result = await controller.DownloadGameFile(key);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Game not found", notFoundResult.Value);
    }
}
