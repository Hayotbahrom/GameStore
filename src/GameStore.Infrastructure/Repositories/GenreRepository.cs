// <copyright file="GenreRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Domain.Entities;
using GameStore.Infrastructure.DbContexts;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

/// <summary>
/// Repository for Genre-specific operations.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="GenreRepository"/> class with the specified context.
/// </remarks>
/// <param name="context">The EF Core database context.</param>
public class GenreRepository(GameStoreDbContext context) : Repository<Genre>(context), IGenreRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Genre>> GetByGameKeyAsync(string gameKey)
    {
        return await Context.Games
            .Where(g => g.Key == gameKey)
            .SelectMany(g => g.GameGenres
                .Where(gg => gg.Genre != null)
                .Select(gg => gg.Genre!))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Genre>> GetByParentIdAsync(Guid parentId)
    {
        return await DbSet
            .Where(g => g.ParentGenreId == parentId)
            .AsNoTracking()
            .ToListAsync();
    }

    /// <inheritdoc/>
    public override async Task<Genre?> FindByIdAsync(Guid id)
    {
        return await Context.Genres
            .Include(g => g.ParentGenre)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}
