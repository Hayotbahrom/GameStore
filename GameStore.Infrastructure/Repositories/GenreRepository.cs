// <copyright file="GenreRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using GameStore.Domain.Entities;
    using GameStore.Infrastructure.DbContexts;
    using GameStore.Infrastructure.IRepositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for Genre-specific operations.
    /// </summary>
    public class GenreRepository : Repository<Genre>, IGenreRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenreRepository"/> class with the specified context.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public GenreRepository(GameStoreDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey)
        {
            return await this.Context.Games
                .Where(g => g.Key == gameKey)
                .SelectMany(g => g.GameGenres
                    .Where(gg => gg.Genre != null)
                    .Select(gg => gg.Genre!))
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId)
        {
            return await this.DbSet
                .Where(g => g.ParentGenreId == parentId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public override async Task<Genre?> FindByIdAsync(Guid id)
        {
            return await this.Context.Genres
                .Include(g => g.ParentGenre)
                .FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
