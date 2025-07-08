// <copyright file="Repository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using GameStore.Infrastructure.DbContexts;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories;

/// <summary>
/// implementation of the Generic Repository using Entity Framework.
/// </summary>
/// <typeparam name="T">the type of the entity . </typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="Repository{T}"/> class.
/// </remarks>
/// <param name="context">The database context.</param>
public class Repository<T>(GameStoreDbContext context) : IRepository<T>
    where T : class
{
    /// <summary>
    /// Gets the entity set for the repository.
    /// </summary>
    protected DbSet<T> DbSet { get; } = context.Set<T>();

    /// <summary>
    /// Gets the database context.
    /// </summary>
    protected GameStoreDbContext Context { get; } = context;

    /// <inheritdoc/>
    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await DbSet
            .Where(predicate)
            .ToListAsync();

    /// <inheritdoc/>
    public virtual async Task<T?> FindByIdAsync(Guid id)
        => await DbSet.FindAsync(id);

    /// <inheritdoc/>
    public virtual IQueryable<T> GetAll()
        => DbSet;

    /// <inheritdoc/>
    public async Task AddAsync(T entity)
        => await DbSet.AddAsync(entity);

    /// <inheritdoc/>
    public void Delete(T entity)
        => DbSet.Remove(entity);

    /// <inheritdoc/>
    public void Update(T entity)
        => DbSet.Update(entity);
}
