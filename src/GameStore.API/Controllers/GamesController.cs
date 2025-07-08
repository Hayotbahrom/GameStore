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
    public async Task<IActionResult> PostAsync(GameCreationDto dto)
        => Ok(await _gameService.AddGameAsync(dto));

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
    public async Task<IActionResult> PutAsync(GameUpdateDto dto)
        => Ok(await _gameService.UpdateGameAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => Ok(await _gameService.DeleteGameAsync(id));

    [HttpGet("by-genre/{genreId}")]
    public async Task<IActionResult> GetByGenreAsync(Guid genreId)
    => Ok(await _gameService.GetByGenreAsync(genreId));

    [HttpGet("by-platform/{platformId}")]
    public async Task<IActionResult> GetByPlatformAsync(Guid platformId)
        => Ok(await _gameService.GetByPlatformAsync(platformId));
}
