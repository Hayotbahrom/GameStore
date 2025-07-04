// <copyright file="GenreController.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using GameStore.Application.DTOs.Genres;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.MVC.Controllers;

/// <summary>
/// Represents the controller for managing genre-related operations in the Game Store application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GenreController"/> class with the specified genre service.
/// </remarks>
/// <param name="genreService">Genre service instance.</param>
/// <param name="gameService">Game service instance.</param>
[Route("genres")]
public class GenreController(
    IGenreService genreService,
    IGameService gameService) : Controller
{
    /// <summary>
    /// The genre service instance used for genre-related operations.
    /// </summary>
    private readonly IGenreService _genreService = genreService;

    /// <summary>
    /// The game service instance used for game-related operations.
    /// </summary>
    private readonly IGameService _gameService = gameService;

    /// <summary>
    /// Renders the index view for the Genre controller.
    /// </summary>
    /// <returns>Index view.</returns>
    [HttpGet("index")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> Index()
    {
        var genres = await _genreService.GetAllAsync();
        return View(genres);
    }

    /// <summary>
    /// Renders the view for creating a new genre.
    /// </summary>
    /// <returns>Create view.</returns>
    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        await PopulateViewBagAsync();
        return View();
    }

    /// <summary>
    /// Represents the create action that processes the creation of a new genre.
    /// </summary>
    /// <param name="dto">GenreForCreation Dto.</param>
    /// <returns>Index view.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(GenreForCreationDto dto)
    {
        if (!ModelState.IsValid)
        {
            View(dto);
            await PopulateViewBagAsync();
        }

        await _genreService.AddAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Deleting genre with given id.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>Index view.</returns>
    [HttpPost("delete/{id}")]
    [ActionName("Delete")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var genre = await _genreService.GetByIdAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        await _genreService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Renders the details view for a specific genre identified by its ID.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>Details view.</returns>
    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var genre = await _genreService.GetByIdAsync(id);
        return genre == null ? NotFound() : View(genre);
    }

    /// <summary>
    /// Renders the edit view for a specific genre identified by its ID.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>Edit view.</returns>
    [HttpGet("edit/{id}")]
    public async Task<IActionResult> Edit(Guid id)
    {
        var genre = await _genreService.GenreForUpdateDtoAsync(id);
        if (genre == null)
        {
            return NotFound();
        }

        await PopulateViewBagAsync();

        return View(genre);
    }

    /// <summary>
    /// Represents the action that processes the editing of a genre.
    /// </summary>
    /// <param name="dto">GenreForUpdate dto.</param>
    /// <returns>Index view.</returns>
    [HttpPost]
    public async Task<IActionResult> Edit(GenreForUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBagAsync();
            return View(dto);
        }

        await _genreService.UpdateAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Retrieves games associated with a specific genre identified by its ID.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>GetGamesByGenre view.</returns>
    [HttpGet("{id}/games")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetGamesByGenre(Guid id)
    {
        var games = await _gameService.GetByGenreAsync(id);
        return View(games);
    }

    /// <summary>
    /// Retrieves sub-genres associated with a specific parent genre identified by its ID.
    /// </summary>
    /// <param name="id">Parent-genre id.</param>
    /// <returns>GetSubgenres view, List of genres.</returns>
    [HttpGet("/genres/{id}/genres")]
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetSubGenres(Guid id)
    {
        var subGenres = await _genreService.GetByParentIdAsync(id);
        return View(subGenres);
    }

    /// <summary>
    /// Populates the ViewBag with genres for dropdown lists.
    /// </summary>
    private async Task PopulateViewBagAsync()
    {
        ViewBag.Genres = new SelectList(
            await _genreService.GetAllAsync(),
            "Id",
            "Name");
    }
}
