// <copyright file="IGenreRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Domain.Entities;

namespace GameStore.Infrastructure.IRepositories;

/// <summary>
/// Represents a genre repository interface for the Game Store application.
/// </summary>
public interface IGenreRepository : IRepository<Genre>
{
    /// <summary>
    /// Gets genres by parent ID asynchronously.
    /// </summary>
    /// <param name="parentId">The parent genre ID.</param>
    /// <returns>List of subgenres.</returns>
    Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId);

    /// <summary>
    /// Gets genres associated with game key asynchronously.
    /// </summary>
    /// <param name="gameKey">Game key.</param>
    /// <returns>List of genres.</returns>
    Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey);

    /// <summary>
    /// Gets existing genre ids.
    /// </summary>
    /// <param name="ids">List of genre ids.</param>
    /// <returns>List of existing genre ids.</returns>
    Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids);
}
