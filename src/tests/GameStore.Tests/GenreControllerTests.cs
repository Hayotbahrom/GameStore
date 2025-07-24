using GameStore.API.Controllers;
using GameStore.Application.DTOs.Genres;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests;
public class GenreControllerTests
{
    private readonly Mock<IGenreService> _mockGenreService;
    private readonly GenresController _controller;

    public GenreControllerTests()
    {
        _mockGenreService = new Mock<IGenreService>();
        _controller = new GenresController(_mockGenreService.Object);
    }

    [Fact]
    public async Task CreateAsyncReturnsOkResultWithCreatedGenre()
    {
        // Arrange
        var dto = new GenreCreationDto
        {
            Name = "Action",
        };

        var expected = new GenreResultDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
        };

        _mockGenreService.Setup(s => s.AddAsync(dto)).ReturnsAsync(expected);

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GenreResultDto>(okResult.Value);

        Assert.Equal(expected.Id, returnValue.Id);
        Assert.Equal(expected.Name, returnValue.Name);
    }

    [Fact]
    public async Task GetAllAsyncReturnsOkResultWithListOfGenres()
    {
        var genres = new List<GenreResultDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Action" },
            new() { Id = Guid.NewGuid(), Name = "Strategy" },
        };

        _mockGenreService.Setup(s => s.GetAllAsync()).ReturnsAsync(genres);

        var result = await _controller.GetAllAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<GenreResultDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetGenresByParentIdAsyncReturnsOkResult()
    {
        // Arrange
        var parentId = Guid.NewGuid();
        var genres = new List<GenreResultDto> { new() { Id = Guid.NewGuid(), Name = "RPG" } };
        _mockGenreService.Setup(s => s.GetByParentIdAsync(parentId)).ReturnsAsync(genres);

        // Act
        var result = await _controller.GetGenresByParentIdAsync(parentId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<GenreResultDto>>(okResult.Value);
        Assert.Single(returnValue);
    }

    [Fact]
    public async Task GetByIdAsyncReturnsOkResult()
    {
        // Arrange
        var genreId = Guid.NewGuid();
        var genre = new GenreResultDto { Id = genreId, Name = "Strategy" };
        _mockGenreService.Setup(s => s.GetByIdAsync(genreId)).ReturnsAsync(genre);

        // Act
        var result = await _controller.GetByIdAsync(genreId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<GenreResultDto>(okResult.Value);
        Assert.Equal("Strategy", returnValue.Name);
    }

    [Fact]
    public async Task UpdateAsyncReturnsOkResult()
    {
        // Arrange
        var dto = new GenreUpdateDto { Id = Guid.NewGuid(), Name = "Updated Genre" };
        _mockGenreService.Setup(s => s.UpdateAsync(dto)).ReturnsAsync(true);

        // Act
        var result = await _controller.PutAsync(dto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var success = Assert.IsType<bool>(okResult.Value);
        Assert.True(success);
    }

    [Fact]
    public async Task DeleteAsyncReturnsOkResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockGenreService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteAsync(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var success = Assert.IsType<bool>(okResult.Value);
        Assert.True(success);
    }
}
