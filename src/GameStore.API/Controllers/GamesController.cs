using GameStore.Application.DTOs.Games;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GamesController(IGameService gameService) : ControllerBase
{
    private readonly IGameService _gameService = gameService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(GameCreationDto dto)
        => Ok(await _gameService.AddAsync(dto));

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _gameService.GetAllAsync());

    [HttpGet("{key}")]
    public async Task<IActionResult> GetByKeyAsync(string key)
        => Ok(await _gameService.GetByKeyAsync(key));

    [HttpGet("find/{id}")]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _gameService.GetByIdAsync(id));

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(GameUpdateDto dto)
        => Ok(await _gameService.UpdateAsync(dto));

    [HttpDelete("{key}")]
    public async Task<IActionResult> DeleteAsync(string key)
        => Ok(await _gameService.DeleteAsync(key));

    [HttpGet("{key}/file")]
    public async Task<IActionResult> DownloadGameFile(string key)
    {
        try
        {
            var fileBytes = await _gameService.GenerateGameFileAsync(key);
            var fileName = $"_{key}.txt";

            return File(fileBytes, "text/plain", fileName);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }
}
