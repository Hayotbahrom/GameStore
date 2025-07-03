// <copyright file="GenreService.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.Services
{
    using AutoMapper;
    using GameStore.Application.DTOs.Genres;
    using GameStore.Application.Interfaces;
    using GameStore.Domain.Entities;
    using GameStore.Infrastructure.IRepositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Service for managing genre-related operations.
    /// </summary>
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance.</param>
        /// <param name="mapper">The mapper instance for mapping between entities and DTOs.</param>
        public GenreService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<Guid> AddAsync(GenreForCreationDto dto)
        {
            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ParentGenreId = dto.ParentGenreId,
            };

            await this.unitOfWork.Genres.AddAsync(genre);
            await this.unitOfWork.SaveChangesAsync();
            return genre.Id;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingGenre = await this.ValidateGenreAsync(id);

            this.unitOfWork.Genres.Delete(existingGenre);
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GenreForResultDto>> GetAllAsync()
        {
            var genres = await this.unitOfWork.Genres
                .GetAll()
                .Include(x => x.ParentGenre)
                .ToListAsync();

            return this.mapper.Map<IEnumerable<GenreForResultDto>>(genres);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GenreForResultDto>> GetByGameKeyAsync(string gameKey)
        {
            var genres = await this.unitOfWork.Genres.GetByGameKeyAsync(gameKey);
            return this.mapper.Map<IEnumerable<GenreForResultDto>>(genres);
        }

        /// <inheritdoc/>
        public async Task<GenreForResultDto?> GetByIdAsync(Guid id)
        {
            var genre = await this.unitOfWork.Genres.FindByIdAsync(id);
            return this.mapper.Map<GenreForResultDto>(genre);
        }

        /// <inheritdoc/>
        public async Task<GenreForUpdateDto?> GenreForUpdateDtoAsync(Guid id)
        {
            var genre = await this.unitOfWork.Genres.FindByIdAsync(id);
            if (genre == null)
            {
                return null;
            }

            return this.mapper.Map<GenreForUpdateDto>(genre);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GenreForResultDto>> GetByParentIdAsync(Guid parentId)
        {
            var genres = await this.unitOfWork.Genres.GetByParentIdAsync(parentId);
            return this.mapper.Map<IEnumerable<GenreForResultDto>>(genres);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateAsync(GenreForUpdateDto dto)
        {
            var existingGenre = await this.ValidateGenreAsync(dto.Id);

            existingGenre.Name = dto.Name;
            existingGenre.ParentGenreId = dto.ParentGenreId;

            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Guid>> GetGenreIdsByGameNamesAsync(IEnumerable<string> gameNames)
        {
            var genres = await this.unitOfWork.Genres.GetAll().ToListAsync();
            return genres
                .Where(g => gameNames.Contains(g.Name))
                .Select(g => g.Id);
        }

        /// <summary>
        /// Validate genre with given id.
        /// </summary>
        /// <param name="genreId">Genre id.</param>
        /// <returns>Genre model.</returns>
        private async Task<Genre> ValidateGenreAsync(Guid genreId)
        {
            var genre = await this.unitOfWork.Genres.FindByIdAsync(genreId);
            if (genre == null)
            {
                throw new ArgumentException($"Game with ID {genreId} does not exist.");
            }

            return genre;
        }
    }
}
