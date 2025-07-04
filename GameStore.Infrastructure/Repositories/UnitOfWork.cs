// <copyright file="UnitOfWork.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using GameStore.Infrastructure.DbContexts;
using GameStore.Infrastructure.IRepositories;

namespace GameStore.Infrastructure.Repositories;

/// <summary>
/// implementation of the Unit of Work.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UnitOfWork"/> class.
/// </remarks>
/// <param name="context">The EF core database context.</param>
/// <param name="gameRepository">Injected game repository.</param>
/// <param name="genreRepository">Injected genre repository.</param>
/// <param name="platformRepository">Injected platform repository.</param>
public class UnitOfWork(
    GameStoreDbContext context,
    IGameRepository gameRepository,
    IGenreRepository genreRepository,
    IPlatformRepository platformRepository) : IUnitOfWork
{
    /// <inheritdoc/>
    public IGameRepository Games { get; } = gameRepository;

    /// <inheritdoc/>
    public IPlatformRepository Platforms { get; } = platformRepository;

    /// <inheritdoc/>
    public IGenreRepository Genres { get; } = genreRepository;

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }
}
