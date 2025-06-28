// <copyright file="IRepository.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Infrastructure.IRepositories
{
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a generic repository interface for the Game Store application.
    /// </summary>
    /// <typeparam name="TEntity"> the type of entity to be managed by the repository.</typeparam>
    public interface IRepository<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>Entity .</returns>
        Task Add(TEntity entity);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Gets all entities from the repository.
        /// </summary>
        /// <returns>A collection of entities.</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Finds an entity by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity.</param>
        /// <returns>The found entity or null if not found.</returns>
        Task<TEntity?> FindByIdAsync(Guid id);

        /// <summary>
        /// Finds entities that match a given condition.
        /// </summary>
        /// <param name="predicate">The expression used to filter entities.</param>
        /// <returns>A collection of matching entities.</returns>
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
