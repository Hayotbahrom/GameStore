// <copyright file="PlatformController.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.MVC.Controllers
{
    using GameStore.Application.DTOs.Games;
    using GameStore.Application.DTOs.Platforms;
    using GameStore.Application.Interfaces;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Represents the controller for managing platform-related operations in the Game Store application.
    /// </summary>
    public class PlatformController : Controller
    {
        /// <summary>
        /// The platform service instance used for platform-related operations.
        /// </summary>
        private readonly IPlatformService platformService;

        /// <summary>
        /// The game service instance used for game-related operations.
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformController"/> class with the specified platform service.
        /// </summary>
        /// <param name="platformService">Platform service instance.</param>
        /// <param name="gameService">Game service instance.</param>
        public PlatformController(
            IPlatformService platformService,
            IGameService gameService)
        {
            this.platformService = platformService;
            this.gameService = gameService;
        }

        /// <summary>
        /// Renders the index view for the Platform controller.
        /// </summary>
        /// <returns>Index view. </returns>
        // GET: PlatformController
        public async Task<ActionResult> Index()
        {
            var platforms = await this.platformService.GetAllPlatformsAsync();
            return this.View(platforms);
        }

        /// <summary>
        /// Renders the details view for a specific plarform id.
        /// </summary>
        /// <param name="id">Platform id.</param>
        /// <returns>Details view. </returns>
        // GET: PlatformController/Details/5
        public async Task<ActionResult> Details(Guid id)
        {
            var platform = await this.platformService.GetPlatformByIdAsync(id);
            if (platform is null)
            {
                return this.NotFound();
            }

            return this.View(platform);
        }

        /// <summary>
        /// Renders the view for creating a new platform.
        /// </summary>
        /// <returns>Create view.</returns>
        public IActionResult Create()
        {
            return this.View();
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
            if (!this.ModelState.IsValid)
            {
                return this.View(dto);
            }

            await this.platformService.CreatePlatformAsync(dto);
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// Renders the view for updating an existing platform.
        /// </summary>
        /// <param name="id">Platform id.</param>
        /// <returns>Edit view.</returns>
        // GET: PlatformController/Edit/5
        public async Task<ActionResult> Edit(Guid id)
        {
            var platform = await this.platformService.GetPlatformByIdAsync(id);
            if (platform is null)
            {
                return this.NotFound();
            }

            var dto = new PlatformForUpdateDto
            {
                Id = platform.Id,
                Type = platform.Type,
            };

            return this.View(dto);
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
                if (!this.ModelState.IsValid)
                {
                    return this.View(dto);
                }

                await this.platformService.UpdatePlatformAsync(dto);
                return this.RedirectToAction(nameof(this.Index));
            }
            catch
            {
                return this.View(dto);
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
            var platform = await this.platformService.GetPlatformByIdAsync(id);
            if (platform is null)
            {
                return this.NotFound();
            }

            await this.platformService.DeletePlatformAsync(id);
            return this.View();
        }

        /// <summary>
        /// Represents the games assoiceated with platform .
        /// </summary>
        /// <param name="id">Platform id.</param>
        /// <returns>GetGamesByPlatform view.</returns>
        public async Task<IActionResult> GetGamesByPlatform(Guid id)
        {
            var games = await this.gameService.GetByPlatformAsync(id);
            return this.View(games);
        }
    }
}
