// <copyright file="GameController.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.MVC.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using GameStore.Application.DTOs.Games;
    using GameStore.Application.Interfaces;
    using GameStore.Infrastructure.IRepositories;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// Represents the controller for managing game-related operations in the Game Store application.
    /// </summary>
    public class GameController : Controller
    {
        /// <summary>
        /// The game service instance used for game-related operations.
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// The unit of work instance used for database operations.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The genre service instance used for genre-related operations.
        /// </summary>
        private readonly IGenreService genreService;

        /// <summary>
        /// The platform service instance used for platform-related operations.
        /// </summary>
        private readonly IPlatformService platformService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameController"/> class with the specified game service.
        /// </summary>
        /// <param name="gameService">GameService instance. </param>
        /// <param name="unitOfWork">UnitOfWork instance.</param>
        /// <param name="genreService">GenreService instance.</param>
        /// <param name="platformService">PlatformService instance.</param>
        public GameController(
            IGameService gameService,
            IUnitOfWork unitOfWork,
            IGenreService genreService,
            IPlatformService platformService)
        {
            this.gameService = gameService;
            this.unitOfWork = unitOfWork;
            this.genreService = genreService;
            this.platformService = platformService;
        }

        /// <summary>
        /// Represents the index action that retrieves all games and returns the view.
        /// </summary>
        /// <param name="key">Key of game for filtering games. </param>
        /// <param name="genreId">Genre ID for filtering games.</param>
        /// <param name="platformId">Platform ID for filtering games.</param>
        /// <returns>Index view. </returns>
        public async Task<IActionResult> Index(string? key, Guid? genreId, Guid? platformId)
        {
            var games = await this.gameService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(key))
            {
                var game = await this.gameService.GetByKeyAsync(key);
                games = game is null ? Enumerable.Empty<GameForResultDto>() : new[] { game };
            }

            // Filter by genre
            if (genreId.HasValue)
            {
                games = await this.gameService.GetByGenreAsync(genreId.Value);
            }

            // Filter by platform
            if (platformId.HasValue)
            {
                games = await this.gameService.GetByPlatformAsync(platformId.Value);
            }

            this.ViewBag.Genres = new SelectList(await this.genreService.GetAllAsync(), "Id", "Name");
            this.ViewBag.Platforms = new SelectList(await this.platformService.GetAllPlatformsAsync(), "Id", "Type");

            return this.View(games);
        }

        /// <summary>
        /// Represents the create action that returns the view for creating a new game.
        /// </summary>
        /// <returns>Create view.</returns>
        public async Task<IActionResult> Create()
        {
            this.ViewBag.Genres = new MultiSelectList(await this.unitOfWork.Genres.GetAll().ToListAsync(), "Id", "Name");
            this.ViewBag.Platforms = new MultiSelectList(await this.unitOfWork.Platforms.GetAll().ToListAsync(), "Id", "Type");

            return this.View();
        }

        /// <summary>
        /// Represents the create action that processes the creation of a new game.
        /// </summary>
        /// <param name="game">GameForCreationDto.</param>
        /// <returns>Index view.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(GameForCreationDto game)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(game);
            }

            await this.gameService.AddGameAsync(game);
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// Represents the details of game given its Id.
        /// </summary>
        /// <param name="id">Game ID.</param>
        /// <returns>GameForResultDto.</returns>
        public async Task<IActionResult> Details(Guid id)
        {
            var game = await this.gameService.GetByIdAsync(id);
            if (game is null)
            {
                return this.NotFound();
            }

            return this.View(game);
        }

        /// <summary>
        /// Represents the edit action that returns the view for editing a game given its Id.
        /// </summary>
        /// <param name="id">Game id.</param>
        /// <returns>GameForResultDto. </returns>
        public async Task<IActionResult> Edit(Guid id)
        {
            var game = await this.gameService.GetByIdAsync(id);
            if (game is null)
            {
                return this.NotFound();
            }

            var dto = new GameForUpdateDto
            {
                Id = game.Id,
                Name = game.Name,
                Key = game.Key,
                Description = game.Description,
                GenreIds = (await this.genreService.GetGenreIdsByGameNamesAsync(game.Genres)).ToList(),
                PlatformIds = (await this.platformService.GetPlatformIdsByGameNamesAsync(game.Platforms)).ToList(),
            };

            this.ViewBag.Genres = new MultiSelectList(await this.unitOfWork.Genres.GetAll().ToListAsync(), "Id", "Name");
            this.ViewBag.Platforms = new MultiSelectList(await this.unitOfWork.Platforms.GetAll().ToListAsync(), "Id", "Type");

            return this.View(dto);
        }

        /// <summary>
        /// Represents the edit action that processes the update of a game given its Id.
        /// </summary>
        /// <param name="dto">Update dto.</param>
        /// <returns>Index view.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(GameForUpdateDto dto)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(dto);
            }

            await this.gameService.UpdateGameAsync(dto);
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// Represents the delete action that processes the deletion of a game given its Id.
        /// </summary>
        /// <param name="id">Game id.</param>
        /// <returns>Index view.</returns>
        public async Task<IActionResult> Delete(Guid id)
        {
            var game = await this.gameService.GetByIdAsync(id);
            if (game is null)
            {
                return this.NotFound();
            }

            await this.gameService.DeleteGameAsync(id);
            return this.RedirectToAction(nameof(this.Index));
        }
    }
}
