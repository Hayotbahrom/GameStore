// <copyright file="IGenreService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using GameStore.Application.DTOs.Games;
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
    Task<IEnumerable<GenreResultDto>> GetAllAsync();

    /// <summary>
    /// Gets all game associated with given genre ID asynchronously.
    /// </summary>
    /// <param name="genreId">The identifier of the genre.</param>
    /// <returns>GameForResult list.</returns>
    Task<IEnumerable<GameResultDto>> GetGamesByGenreIdAsync(Guid genreId);

    /// <summary>
    /// Gets a genre by its identifier asynchronously.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>GenreForResultDto.</returns>
    Task<GenreResultDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Gets sub-genres associated with a specific parent genre ID asynchronously.
    /// </summary>
    /// <param name="parentId">Parent genre id.</param>
    /// <returns>List of GenreFoResultDto.</returns>
    Task<IEnumerable<GenreResultDto>> GetByParentIdAsync(Guid parentId);

    /// <summary>
    /// Adds a new genre asynchronously.
    /// </summary>
    /// <param name="dto">GenreForCreation DTO.</param>
    /// <returns>New genre id.</returns>
    Task<GenreResultDto> AddAsync(GenreCreationDto dto);

    /// <summary>
    /// Updates an existing genre asynchronously.
    /// </summary>
    /// <param name="dto">GenreForUpdateDto. </param>
    /// <returns>bool.</returns>
    Task<bool> UpdateAsync(GenreUpdateDto dto);

    /// <summary>
    /// Deletes a genre by its identifier asynchronously.
    /// </summary>
    /// <param name="id">Genre id.</param>
    /// <returns>bool.</returns>
    Task<bool> DeleteAsync(Guid id);
}
