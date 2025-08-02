using GameStore.Application.DTOs.Genres;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenresController(IGenreService genreService) : ControllerBase
{
    private readonly IGenreService _genreService = genreService;

    [HttpPost]
    public async Task<IActionResult> CreateAsync(GenreCreationDto dto)
        => Ok(await _genreService.AddAsync(dto));

    [HttpGet]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetAllAsync()
        => Ok(await _genreService.GetAllAsync());

    [HttpGet("{id}/genres")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetGenresByParentIdAsync(Guid id)
        => Ok(await _genreService.GetByParentIdAsync(id));

    [HttpGet("{id}/games")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetGamesByGenreIdAsync(Guid id)
        => Ok(await _genreService.GetGamesByGenreIdAsync(id));

    [HttpGet("{id}")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
    public async Task<IActionResult> GetByIdAsync(Guid id)
        => Ok(await _genreService.GetByIdAsync(id));

    [HttpPut]
    public async Task<IActionResult> PutAsync(GenreUpdateDto dto)
        => Ok(await _genreService.UpdateAsync(dto));

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
        => Ok(await _genreService.DeleteAsync(id));
}
