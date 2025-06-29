// <copyright file="IUnitOfWork.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Infrastructure.IRepositories
{
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a unit of work interface for the Game Store application.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Gets the repository for managing games.
        /// </summary>
        IGameRepository Games { get; }

        /// <summary>
        /// Gets the repository for managing platforms.
        /// </summary>
        IPlatformRepository Platforms { get; }

        /// <summary>
        /// Gets the repository for managing genres.
        /// </summary>
        IGenreRepository Genres { get; }

        /// <summary>
        /// Asynchronously saves changes to the database.
        /// </summary>
        /// <returns>An integer representing the number of state entries written to database. </returns>
        Task<int> SaveChangesAsync();
    }
}
