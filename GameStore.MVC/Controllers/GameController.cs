// <copyright file="GameController.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Application.DTOs.Games;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GameStore.MVC.Controllers;

/// <summary>
/// Represents the controller for managing game-related operations in the Game Store application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GameController"/> class with the specified game service.
/// </remarks>
/// <param name="gameService">GameService instance. </param>
/// <param name="genreService">GenreService instance.</param>
/// <param name="platformService">PlatformService instance.</param>
public class GameController(IGameService gameService, IGenreService genreService, IPlatformService platformService) : Controller
{
    /// <summary>
    /// The game service instance used for game-related operations.
    /// </summary>
    private readonly IGameService _gameService = gameService;

    /// <summary>
    /// The genre service instance used for genre-related operations.
    /// </summary>
    private readonly IGenreService _genreService = genreService;

    /// <summary>
    /// The platform service instance used for platform-related operations.
    /// </summary>
    private readonly IPlatformService _platformService = platformService;

    /// <summary>
    /// Represents the index action that retrieves all games and returns the view.
    /// </summary>
    /// <param name="key">Key of game for filtering games. </param>
    /// <param name="genreId">Genre ID for filtering games.</param>
    /// <param name="platformId">Platform ID for filtering games.</param>
    /// <returns>Index view. </returns>
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<IActionResult> Index(string? key, Guid? genreId, Guid? platformId)
    {
        var games = await _gameService.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(key))
        {
            var game = await _gameService.GetByKeyAsync(key);
#pragma warning disable IDE0028
            games = game is null ? new List<GameForResultDto>() : new List<GameForResultDto> { game };
#pragma warning restore IDE0028
        }

        if (genreId.HasValue)
        {
            games = await _gameService.GetByGenreAsync(genreId.Value);
        }

        if (platformId.HasValue)
        {
            games = await _gameService.GetByPlatformAsync(platformId.Value);
        }

        ViewBag.Genres = new SelectList(
            await _genreService.GetAllAsync(),
            "Id",
            "Name");

        ViewBag.Platforms = new SelectList(
            await _platformService.GetAllPlatformsAsync(),
            "Id",
            "Type");

        return View(games);
    }

    /// <summary>
    /// Represents the create action that returns the view for creating a new game.
    /// </summary>
    /// <returns>Create view.</returns>
    public async Task<IActionResult> Create()
    {
        await PopulateViewBagAsync();
        return View();
    }

    /// <summary>
    /// Represents the create action that processes the creation of a new game.
    /// </summary>
    /// <param name="game">GameForCreationDto.</param>
    /// <returns>Index view.</returns>
    [HttpPost]
    public async Task<IActionResult> Create(GameForCreationDto game)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBagAsync();
            return View(game);
        }

        await _gameService.AddGameAsync(game);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Represents the details of game given its Id.
    /// </summary>
    /// <param name="id">Game ID.</param>
    /// <returns>GameForResultDto.</returns>
    public async Task<IActionResult> Details(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        return game is null ? NotFound() : View(game);
    }

    /// <summary>
    /// Represents the edit action that returns the view for editing a game given its Id.
    /// </summary>
    /// <param name="id">Game id.</param>
    /// <returns>GameForResultDto. </returns>
    public async Task<IActionResult> Edit(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game is null)
        {
            return NotFound();
        }

        var dto = new GameForUpdateDto
        {
            Id = game.Id,
            Name = game.Name,
            Key = game.Key,
            Description = game.Description,
            GenreIds = (await _genreService.GetGenreIdsByGameNamesAsync(game.Genres)).ToList(),
            PlatformIds = (await _platformService.GetPlatformIdsByGameNamesAsync(game.Platforms)).ToList(),
        };

        await PopulateViewBagAsync();

        return View(dto);
    }

    /// <summary>
    /// Represents the edit action that processes the update of a game given its Id.
    /// </summary>
    /// <param name="dto">Update dto.</param>
    /// <returns>Index view.</returns>
    [HttpPost]
    public async Task<IActionResult> Edit(GameForUpdateDto dto)
    {
        if (!ModelState.IsValid)
        {
            await PopulateViewBagAsync();
            return View(dto);
        }

        await _gameService.UpdateGameAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Represents the delete action that processes the deletion of a game given its Id.
    /// </summary>
    /// <param name="id">Game id.</param>
    /// <returns>Index view.</returns>
    public async Task<IActionResult> Delete(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (game is null)
        {
            return NotFound();
        }

        await _gameService.DeleteGameAsync(id);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Downloads the game details as a text file given its Id.
    /// </summary>
    /// <param name="id">Game id.</param>
    /// <returns>Details file.</returns>
    [HttpGet]
    public async Task<IActionResult> Download(Guid id)
    {
        var game = await _gameService.GetByIdAsync(id);
        if (!ModelState.IsValid)
        {
            return NotFound();
        }

        var content = $"Name: {game.Name}\nKey: {game.Key}\nDescription: {game.Description}";
        var bytes = System.Text.Encoding.UTF8.GetBytes(content);
        var fileName = $"_{game.Key}.txt";

        return File(bytes, "text/plain", fileName);
    }

    /// <summary>
    /// Populates the ViewBag with genres and platforms for use in views.
    /// </summary>
    private async Task PopulateViewBagAsync()
    {
        ViewBag.Genres = new MultiSelectList(
            await _genreService.GetAllAsync(),
            "Id",
            "Name");

        ViewBag.Platforms = new MultiSelectList(
            await _platformService.GetAllPlatformsAsync(),
            "Id",
            "Type");
    }
}
