// <copyright file="GameRepository.cs" company="GameStore">
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
    /// Repository for game-specific operations, extending the generic repository.
    /// </summary>
    public class GameRepository : Repository<Game>, IGameRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameRepository"/> class.
        /// </summary>
        /// <param name="context">The EF core database context. </param>
        public GameRepository(GameStoreDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<Game?> GetByKeyAsync(string key)
        {
            return await this.DbSet.FirstOrDefaultAsync(g => g.Key == key);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId)
        {
            return await this.DbSet
                .Where(g => g.GameGenres != null && g.GameGenres.Any(gg => gg.GenreId == genreId))
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId)
        {
            return await this.DbSet
                .Where(g => g.GamePlatforms != null && g.GamePlatforms.Any(gp => gp.PlatformId == platformId))
                .AsNoTracking()
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Game?> GetFullGameByIdAsync(Guid id)
        {
            return await this.DbSet
                .Include(gg => gg.GameGenres)
                    .ThenInclude(g => g.Genre)
                .Include(gp => gp.GamePlatforms)
                    .ThenInclude(p => p.Platform)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == id);
        }
    }
}
