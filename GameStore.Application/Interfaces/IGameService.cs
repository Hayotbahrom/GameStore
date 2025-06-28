// <copyright file="IGameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameStore.Application.DTOs;

    /// <summary>
    /// Provides an interface for game services.
    /// </summary>
    public interface IGameService
    {
        /// <summary>
        /// Gets all games asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation, containing a list of games.</returns>
        Task<IEnumerable<GameForResultDto>> GetAllAsync();

        /// <summary>
        /// Gets a game by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the game.</param>
        /// <returns>A task that represents the asynchronous operation, containing the game.</returns>
        Task<GameForResultDto> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a game by key asynchronously.
        /// </summary>
        /// <param name="key">The key of the game.</param>
        /// <returns>A task that represents the asynchronous operation, containing the game.</returns>
        Task<GameForResultDto> GetByKeyAsync(string key);

        /// <summary>
        /// Gets all game associated with given genre ID asynchronously.
        /// </summary>
        /// <param name="genreId">The identifier of the genre.</param>
        /// <returns>GameForResult list.</returns>
        Task<IEnumerable<GameForResultDto>> GetByGenreAsync(Guid genreId);

        /// <summary>
        /// Gets all game associated with given platform ID asynchronously.
        /// </summary>
        /// <param name="platformId">The identifier of the platform.</param>
        /// <returns>GameForResult list.</returns>
        Task<IEnumerable<GameForResultDto>> GetByPlatformAsync(Guid platformId);

        /// <summary>
        /// Adds a new game asynchronously.
        /// </summary>
        /// <param name="game">The game to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddGameAsync(GameForCreationDto game);

        /// <summary>
        /// Updates an existing game asynchronously.
        /// </summary>
        /// <param name="game">The game to update.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<bool> UpdateGameAsync(GameForUpdateDto game);

        /// <summary>
        /// Deletes a game by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the game to delete.</param>
        /// <returns>The task result is true if the game was deleted, otherwise false.</returns>
        Task<bool> DeleteGameAsync(Guid id);

        /// <summary>
        ///  Generates a downloadable text file containing serialized game data by its key asynchronously.
        /// </summary>
        /// <param name="key">The unique game key used to find the game.</param>
        /// <returns>A byte array representing the content of the game file.</returns>
        Task<byte[]> GenerateGameFileAsync(string key);
    }
}
