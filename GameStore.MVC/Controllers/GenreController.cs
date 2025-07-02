// <copyright file="GenreController.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.MVC.Controllers
{
    using GameStore.Application.DTOs.Genres;
    using GameStore.Application.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;

    /// <summary>
    /// Represents the controller for managing genre-related operations in the Game Store application.
    /// </summary>
    [Route("genres")]
    public class GenreController : Controller
    {
        /// <summary>
        /// The genre service instance used for genre-related operations.
        /// </summary>
        private readonly IGenreService genreService;

        /// <summary>
        /// The game service instance used for game-related operations.
        /// </summary>
        private readonly IGameService gameService;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreController"/> class with the specified genre service.
        /// </summary>
        /// <param name="genreService">Genre service instance.</param>
        /// <param name="gameService">Game service instance.</param>
        public GenreController(
            IGenreService genreService,
            IGameService gameService)
        {
            this.gameService = gameService;
            this.genreService = genreService;
        }

        /// <summary>
        /// Renders the index view for the Genre controller.
        /// </summary>
        /// <returns>Index view.</returns>
        [HttpGet("index")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client, NoStore = false)]
        public async Task<IActionResult> Index()
        {
            var genres = await this.genreService.GetAllAsync();
            return this.View(genres);
        }

        /// <summary>
        /// Renders the view for creating a new genre.
        /// </summary>
        /// <returns>Create view.</returns>
        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            await this.PopulateViewBagAsync();
            return this.View();
        }

        /// <summary>
        /// Represents the create action that processes the creation of a new genre.
        /// </summary>
        /// <param name="dto">GenreForCreation Dto.</param>
        /// <returns>Index view.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(GenreForCreationDto dto)
        {
            if (!this.ModelState.IsValid)
            {
                this.View(dto);
                await this.PopulateViewBagAsync();
            }

            await this.genreService.AddAsync(dto);
            return this.RedirectToAction(nameof(this.Index));
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
            var genre = await this.genreService.GetByIdAsync(id);
            if (genre == null)
            {
                return this.NotFound();
            }

            await this.genreService.DeleteAsync(id);
            return this.RedirectToAction(nameof(this.Index));
        }

        /// <summary>
        /// Renders the details view for a specific genre identified by its ID.
        /// </summary>
        /// <param name="id">Genre id.</param>
        /// <returns>Details view.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            var genre = await this.genreService.GetByIdAsync(id);
            if (genre == null)
            {
                return this.NotFound();
            }

            return this.View(genre);
        }

        /// <summary>
        /// Renders the edit view for a specific genre identified by its ID.
        /// </summary>
        /// <param name="id">Genre id.</param>
        /// <returns>Edit view.</returns>
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var genre = await this.genreService.GenreForUpdateDtoAsync(id);
            if (genre == null)
            {
                return this.NotFound();
            }

            await this.PopulateViewBagAsync();

            return this.View(genre);
        }

        /// <summary>
        /// Represents the action that processes the editing of a genre.
        /// </summary>
        /// <param name="dto">GenreForUpdate dto.</param>
        /// <returns>Index view.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(GenreForUpdateDto dto)
        {
            if (!this.ModelState.IsValid)
            {
                await this.PopulateViewBagAsync();
                return this.View(dto);
            }

            await this.genreService.UpdateAsync(dto);
            return this.RedirectToAction(nameof(this.Index));
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
            var games = await this.gameService.GetByGenreAsync(id);
            return this.View(games);
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
            var subGenres = await this.genreService.GetByParentIdAsync(id);
            return this.View(subGenres);
        }

        /// <summary>
        /// Populates the ViewBag with genres for dropdown lists.
        /// </summary>
        private async Task PopulateViewBagAsync()
        {
            this.ViewBag.Genres = new SelectList(
                await this.genreService.GetAllAsync(),
                "Id",
                "Name");
        }
    }
}
