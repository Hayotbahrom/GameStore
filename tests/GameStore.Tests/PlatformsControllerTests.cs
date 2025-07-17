using GameStore.API.Controllers;
using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace GameStore.Tests;
public class PlatformsControllerTests
{
    private readonly Mock<IPlatformService> _mockPlatformService;
    private readonly PlatformsController _controller;

    public PlatformsControllerTests()
    {
        _mockPlatformService = new Mock<IPlatformService>();
        _controller = new PlatformsController(_mockPlatformService.Object);
    }

    [Fact]
    public async Task CreateAsyncReturnsOkResultWithPlatform()
    {
        var dto = new PlatformCreationDto { Type = "PC" };
        var created = new PlatformResultDto { Id = Guid.NewGuid(), Type = "PC" };

        _mockPlatformService.Setup(s => s.CreateAsync(dto)).ReturnsAsync(created);

        var result = await _controller.CreateAsync(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PlatformResultDto>(okResult.Value);
        Assert.Equal(dto.Type, returnValue.Type);
    }

    [Fact]
    public async Task GetAllAsyncReturnsOkResultWithListOfPlatforms()
    {
        var platforms = new List<PlatformResultDto>
        {
            new() { Id = Guid.NewGuid(), Type = "PC" },
            new() { Id = Guid.NewGuid(), Type = "Xbox" },
        };

        _mockPlatformService.Setup(s => s.GetAllAsync()).ReturnsAsync(platforms);

        var result = await _controller.GetAllAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsAssignableFrom<IEnumerable<PlatformResultDto>>(okResult.Value);
        Assert.Equal(2, returnValue.Count());
    }

    [Fact]
    public async Task GetByIdAsyncReturnsOkResultWithPlatform()
    {
        var id = Guid.NewGuid();
        var platform = new PlatformResultDto { Id = id, Type = "PC" };

        _mockPlatformService.Setup(s => s.GetByIdAsync(id)).ReturnsAsync(platform);

        var result = await _controller.GetByIdAsync(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PlatformResultDto>(okResult.Value);
        Assert.Equal(id, returnValue.Id);
    }

    [Fact]
    public async Task UpdateAsyncReturnsOkResultWithUpdatedPlatform()
    {
        var updateDto = new PlatformUpdateDto { Id = Guid.NewGuid(), Type = "Updated Platform" };

        _mockPlatformService.Setup(s => s.UpdateAsync(updateDto)).ReturnsAsync(true);

        var result = await _controller.UpdateAsync(updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<bool>(okResult.Value);
        Assert.True(returnValue);
    }

    [Fact]
    public async Task DeleteAsyncReturnsOkResultWithTrue()
    {
        var id = Guid.NewGuid();

        _mockPlatformService.Setup(s => s.DeleteAsync(id)).ReturnsAsync(true);

        var result = await _controller.DeleteAsync(id);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.True((bool)okResult.Value!);
    }
}
