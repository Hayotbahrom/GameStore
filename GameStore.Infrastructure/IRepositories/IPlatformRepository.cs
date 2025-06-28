// <copyright file="IPlatformRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Infrastructure.IRepositories
{
    using GameStore.Domain.Entities;

    /// <summary>
    /// represents a platform repository interface for the Game Store application.
    /// </summary>
    public interface IPlatformRepository
    {
        /// <summary>
        /// Gets all platforms associated with game-key asynchronously.
        /// </summary>
        /// <param name="gameKey">Game key.</param>
        /// <returns>List of platforms.</returns>
        Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey);
    }
}
