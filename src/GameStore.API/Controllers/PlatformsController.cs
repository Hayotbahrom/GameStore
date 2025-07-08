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
    public async Task<IActionResult> PostAsync(PlatformCreationDto dto)
        => Ok(await _platformService.CreatePlatformAsync(dto));

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _platformService.GetAllPlatformsAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _platformService.GetPlatformByIdAsync(id));

    [HttpPut]
    public async Task<IActionResult> PutAsync(PlatformUpdateDto dto)
        => Ok(await _platformService.UpdatePlatformAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => Ok(await _platformService.DeletePlatformAsync(id));
}
