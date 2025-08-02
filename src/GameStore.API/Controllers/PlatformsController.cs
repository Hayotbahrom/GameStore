using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlatformsController(IPlatformService platformService) : ControllerBase
{
    private readonly IPlatformService _platformService = platformService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(PlatformCreationDto dto)
        => Ok(await _platformService.CreateAsync(dto));

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _platformService.GetAllAsync());

    [HttpGet("{id}/games")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetGamesByPlatformIdAsync(Guid id)
    => Ok(await _platformService.GetGamesByPlatformIdAsync(id));

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _platformService.GetByIdAsync(id));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(PlatformUpdateDto dto)
        => Ok(await _platformService.UpdateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => Ok(await _platformService.DeleteAsync(id));
}
