// <copyright file="GameRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Domain.Entities;
using GameStore.Infrastructure.DbContexts;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

/// <summary>
/// Repository for game-specific operations, extending the generic repository.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GameRepository"/> class.
/// </remarks>
/// <param name="context">The EF core database context. </param>
public class GameRepository(GameStoreDbContext context) : Repository<Game>(context), IGameRepository
{
    /// <inheritdoc/>
    public override IQueryable<Game> GetAll()
    {
        return DbSet
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform);
    }

    /// <inheritdoc/>
    public async Task<Game?> GetByKeyAsync(string key)
    {
        return await DbSet
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
             .FirstOrDefaultAsync(g => g.Key == key);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Game>> GetByGenreAsync(Guid genreId)
    {
        return await DbSet
            .Where(g => g.GameGenres != null && g.GameGenres.Any(gg => gg.GenreId == genreId))
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Game>> GetByPlatformAsync(Guid platformId)
    {
        return await DbSet
            .Where(g => g.GamePlatforms != null && g.GamePlatforms.Any(gp => gp.PlatformId == platformId))
            .Include(g => g.GameGenres)
                .ThenInclude(gg => gg.Genre)
            .Include(g => g.GamePlatforms)
                .ThenInclude(gp => gp.Platform)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Game?> GetFullGameByIdAsync(Guid id)
    {
        return await DbSet
            .Include(gg => gg.GameGenres)
                .ThenInclude(g => g.Genre)
            .Include(gp => gp.GamePlatforms)
                .ThenInclude(p => p.Platform)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
