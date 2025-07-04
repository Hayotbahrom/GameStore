// <copyright file="IPlatformRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Domain.Entities;

namespace GameStore.Infrastructure.IRepositories;

/// <summary>
/// represents a platform repository interface for the Game Store application.
/// </summary>
public interface IPlatformRepository : IRepository<Platform>
{
    /// <summary>
    /// Gets all platforms associated with game-key asynchronously.
    /// </summary>
    /// <param name="gameKey">Game key.</param>
    /// <returns>List of platforms.</returns>
    Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey);
}
