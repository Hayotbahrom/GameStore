// <copyright file="IGameRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Infrastructure.IRepositories
{
    using GameStore.Domain.Entities;

    /// <summary>
    /// Interface for Game specific repository operations.
    /// </summary>
    public interface IGameRepository : IRepository<Game>
    {
        /// <summary>
        /// Gets a game by its unique key asynchronously.
        /// </summary>
        /// <param name="key">The game's key. </param>
        /// <returns>The matching game.</returns>
        Task<Game?> GetByKeyAsync(string key);

        /// <summary>
        /// Gets all games associated with given genre ID asynchronously.
        /// </summary>
        /// <param name="genreId">Genre ID.</param>
        /// <returns>Game list. </returns>
        Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId);

        /// <summary>
        /// Gets all games associated with given platform ID asynchronously.
        /// </summary>
        /// <param name="platformId">Platform ID.</param>
        /// <returns>Game list.</returns>
        Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId);

        /// <summary>
        /// Loads a full game entity by its ID, including related entities like genres and platforms.
        /// </summary>
        /// <param name="id">Game ID.</param>
        /// <returns>Full game object.</returns>
        Task<Game?> GetFullGameByIdAsync(Guid id);
    }
}
