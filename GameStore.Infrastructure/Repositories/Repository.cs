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
        /// The database context.
        /// </summary>
        protected readonly GameStoreDbContext context;

        /// <summary>
        /// The entity set .
        /// </summary>
        private readonly DbSet<T> dbSet;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="gameStoreDbContext">Database context. </param>
        public Repository(GameStoreDbContext gameStoreDbContext)
        {
            this.context = gameStoreDbContext;
            this.dbSet = this.context.Set<T>();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
            => await this.dbSet
                .Where(predicate)
                .AsNoTracking()
                .ToListAsync();

        /// <inheritdoc/>
        public async Task<T?> FindByIdAsync(Guid id)
            => await this.dbSet.FindAsync(id);

        /// <inheritdoc/>
        public IQueryable<T> GetAll()
            => this.dbSet.AsNoTracking();

        /// <inheritdoc/>
        public async Task Add(T entity)
            => await this.dbSet.AddAsync(entity);

        /// <inheritdoc/>
        public void Delete(T entity)
            => this.dbSet.Remove(entity);

        /// <inheritdoc/>
        public void Update(T entity)
            => this.dbSet.Update(entity);
    }
}
