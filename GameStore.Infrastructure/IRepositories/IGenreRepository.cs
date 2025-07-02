// <copyright file="IGenreRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Infrastructure.IRepositories
{
    using GameStore.Domain.Entities;

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
    }
}
