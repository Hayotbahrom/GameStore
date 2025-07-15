// <copyright file="PlatformRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Domain.Entities;
using GameStore.Infrastructure.DbContexts;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

/// <summary>
/// Repository for managing platform data in the GameStore application.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="PlatformRepository"/> class with the specified database context.
/// </remarks>
/// <param name="context">The EF Core database context.</param>
public class PlatformRepository(GameStoreDbContext context) : Repository<Platform>(context), IPlatformRepository
{
    /// <inheritdoc/>
    public async Task<IEnumerable<Platform>> GetByGameKeyAsync(string gameKey)
    {
        return await Context.Games
            .Where(g => g.Key == gameKey)
            .SelectMany(g => g.GamePlatforms
                .Where(gp => gp.Platform != null)
                .Select(gp => gp.Platform!))
            .ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids)
    {
        return await Context.Genres
            .Where(g => ids.Contains(g.Id))
            .Select(g => g.Id)
            .ToListAsync();
    }
}
