// <copyright file="PlatformController.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.MVC.Controllers;

/// <summary>
/// Represents the controller for managing platform-related operations in the Game Store application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlatformController"/> class with the specified platform service.
/// </remarks>
/// <param name="platformService">Platform service instance.</param>
/// <param name="gameService">Game service instance.</param>
public class PlatformController(
    IPlatformService platformService,
    IGameService gameService) : Controller
{
    /// <summary>
    /// The platform service instance used for platform-related operations.
    /// </summary>
    private readonly IPlatformService _platformService = platformService;

    /// <summary>
    /// The game service instance used for game-related operations.
    /// </summary>
    private readonly IGameService _gameService = gameService;

    /// <summary>
    /// Renders the index view for the Platform controller.
    /// </summary>
    /// <returns>Index view. </returns>
    // GET: PlatformController
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
    public async Task<ActionResult> Index()
    {
        var platforms = await _platformService.GetAllPlatformsAsync();
        return View(platforms);
    }

    /// <summary>
    /// Renders the details view for a specific plarform id.
    /// </summary>
    /// <param name="id">Platform id.</param>
    /// <returns>Details view. </returns>
    // GET: PlatformController/Details/5
    public async Task<ActionResult> Details(Guid id)
    {
        var platform = await _platformService.GetPlatformByIdAsync(id);
        return platform is null ? NotFound() : View(platform);
    }

    /// <summary>
    /// Renders the view for creating a new platform.
    /// </summary>
    /// <returns>Create view.</returns>
    public IActionResult Create()
    {
        return View();
    }

    /// <summary>
    /// Represents creating new platform.
    /// </summary>
    /// <param name="dto">PlatformForCreation dto.</param>
    /// <returns>Index view.</returns>
    // POST: PlatformController/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Create(PlatformForCreationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return View(dto);
        }

        await _platformService.CreatePlatformAsync(dto);
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Renders the view for updating an existing platform.
    /// </summary>
    /// <param name="id">Platform id.</param>
    /// <returns>Edit view.</returns>
    // GET: PlatformController/Edit/5
    public async Task<ActionResult> Edit(Guid id)
    {
        var platform = await _platformService.GetPlatformByIdAsync(id);
        if (platform is null)
        {
            return NotFound();
        }

        var dto = new PlatformForUpdateDto
        {
            Id = platform.Id,
            Type = platform.Type,
        };

        return View(dto);
    }

    /// <summary>
    /// Represents the action that processes the update of an existing platform.
    /// </summary>
    /// <param name="dto">PlatformForUpdate dto.</param>
    /// <returns>Index view.</returns>
    // POST: PlatformController/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(PlatformForUpdateDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            await _platformService.UpdatePlatformAsync(dto);
            return RedirectToAction(nameof(Index));
        }
        catch
        {
            return View(dto);
        }
    }

    /// <summary>
    /// Renders the view for deleting a platform by its ID.
    /// </summary>
    /// <param name="id">Platform id.</param>
    /// <returns>Index view.</returns>
    // GET: PlatformController/Delete/5
    public async Task<ActionResult> Delete(Guid id)
    {
        var platform = await _platformService.GetPlatformByIdAsync(id);
        if (platform is null)
        {
            return NotFound();
        }

        await _platformService.DeletePlatformAsync(id);
        return View();
    }

    /// <summary>
    /// Represents the games assoiceated with platform .
    /// </summary>
    /// <param name="id">Platform id.</param>
    /// <returns>GetGamesByPlatform view.</returns>
    [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetGamesByPlatform(Guid id)
    {
        var games = await _gameService.GetByPlatformAsync(id);
        return View(games);
    }
}
