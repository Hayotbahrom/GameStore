// <copyright file="UnitOfWork.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GameStore.Infrastructure.DbContexts;
    using GameStore.Infrastructure.IRepositories;

    /// <summary>
    /// implementation of the Unit of Work.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        /// <summary>
        /// Represents a unit of work for the Game Store application, managing repositories and database context.
        /// </summary>
        private readonly GameStoreDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="context">The EF core database context.</param>
        /// <param name="gameRepository">Injected game repository.</param>
        /// <param name="genreRepository">Injected genre repository.</param>
        /// <param name="platformRepository">Injected platform repository.</param>
        public UnitOfWork(
            GameStoreDbContext context,
            IGameRepository gameRepository,
            IGenreRepository genreRepository,
            IPlatformRepository platformRepository)
        {
            this.context = context;
            this.GameRepository = gameRepository;
            this.GenreRepository = genreRepository;
            this.PlatformRepository = platformRepository;
        }

        /// <inheritdoc/>
        public IGameRepository GameRepository { get; }

        /// <inheritdoc/>
        public IPlatformRepository PlatformRepository { get; }

        /// <inheritdoc/>
        public IGenreRepository GenreRepository { get; }

        /// <inheritdoc/>
        public async Task<int> SaveChangesAsync()
        {
            return await this.context.SaveChangesAsync();
        }
    }
}
