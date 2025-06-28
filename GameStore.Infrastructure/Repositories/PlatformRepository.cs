// <copyright file="PlatformRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Infrastructure.Repositories
{
    using GameStore.Domain.Entities;
    using GameStore.Infrastructure.DbContexts;
    using GameStore.Infrastructure.IRepositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository for managing platform data in the GameStore application.
    /// </summary>
    public class PlatformRepository : Repository<Platform>, IPlatformRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PlatformRepository"/> class with the specified database context.
        /// </summary>
        /// <param name="context">The EF Core database context.</param>
        public PlatformRepository(GameStoreDbContext context)
            : base(context)
        {
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey)
        {
            return await this.Context.Games
                .Where(g => g.Key == gameKey)
                .SelectMany(g => g.GamePlatforms
                    .Where(gp => gp.Platform != null)
                    .Select(gp => gp.Platform!))
                .ToListAsync();
        }
    }
}
