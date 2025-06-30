// <copyright file="Repository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Infrastructure.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using GameStore.Infrastructure.DbContexts;
    using GameStore.Infrastructure.IRepositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// implementation of the Generic Repository using Entity Framework.
    /// </summary>
    /// <typeparam name="T">the type of the entity . </typeparam>
    public class Repository<T> : IRepository<T>
        where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        public Repository(GameStoreDbContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<T>();
        }

        /// <summary>
        /// Gets the entity set for the repository.
        /// </summary>
        protected DbSet<T> DbSet { get; }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected GameStoreDbContext Context { get; }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await this.DbSet
                .Where(predicate)
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<T?> FindByIdAsync(Guid id)
            => await this.DbSet.FindAsync(id);

        /// <inheritdoc/>
        public virtual IQueryable<T> GetAll()
            => this.DbSet;

        /// <inheritdoc/>
        public async Task AddAsync(T entity)
            => await this.DbSet.AddAsync(entity);

        /// <inheritdoc/>
        public void Delete(T entity)
            => this.DbSet.Remove(entity);

        /// <inheritdoc/>
        public void Update(T entity)
            => this.DbSet.Update(entity);
    }
}
