// <copyright file="IGenreService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using GameStore.Application.DTOs.Genres;

namespace GameStore.Application.Interfaces;

/// <summary>
/// Provides an interface for genre services.
/// </summary>
public interface IGenreService
{
    /// <summary>
    /// Gets all genres asynchronously.
    /// </summary>
    /// <returns>List of GenreFoResultDto.</returns>
    Task<IEnumerable<GenreForResultDto>> GetAllAsync();

    /// <summary>
    /// Gets a genre by its identifier asynchronously.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>GenreForResultDto.</returns>
    Task<GenreForResultDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets sub-genres associated with a specific parent genre ID asynchronously.
    /// </summary>
    /// <param name="parentId">Parent genre id.</param>
    /// <returns>List of GenreFoResultDto.</returns>
    Task<IEnumerable<GenreForResultDto>> GetByParentIdAsync(Guid parentId);

    /// <summary>
    /// Gets genres associated with a specific game key asynchronously.
    /// </summary>
    /// <param name="gameKey">Game key.</param>
    /// <returns>List of games by given gamekey.</returns>
    Task<IEnumerable<GenreForResultDto>> GetByGameKeyAsync(string gameKey);

    /// <summary>
    /// Adds a new genre asynchronously.
    /// </summary>
    /// <param name="dto">GenreForCreation DTO.</param>
    /// <returns>New genre id.</returns>
    Task<Guid> AddAsync(GenreForCreationDto dto);

    /// <summary>
    /// Updates an existing genre asynchronously.
    /// </summary>
    /// <param name="dto">GenreForUpdateDto. </param>
    /// <returns>bool.</returns>
    Task<bool> UpdateAsync(GenreForUpdateDto dto);

    /// <summary>
    /// Deletes a genre by its identifier asynchronously.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>bool.</returns>
    Task<bool> DeleteAsync(Guid id);

    /// <summary>
    /// Gets genre IDs associated with specific game names.
    /// </summary>
    /// <param name="gameNames">List of game names.</param>
    /// <returns>List of genre ids.</returns>
    Task<IEnumerable<Guid>> GetGenreIdsByGameNamesAsync(IEnumerable<string> gameNames);
}
