using AutoMapper;
using GameStore.Application.DTOs.Genres;
using GameStore.Application.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Application.Services
{
    public class GenreService : IGenreService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public GenreService(
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Guid> AddAsync(GenreForCreationDto dto)
        {
            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ParentGenreId = dto.ParentGenreId
            };

            await this.unitOfWork.Genres.AddAsync(genre);
            await this.unitOfWork.SaveChangesAsync();
            return genre.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingGenre = await this.unitOfWork.Genres.FindByIdAsync(id);
            if (existingGenre == null)
            {
                return false; // Genre not found
            }
        
            this.unitOfWork.Genres.Delete(existingGenre);
            await this.unitOfWork.SaveChangesAsync();
            return true; // Deletion successful
        }

        public async Task<IEnumerable<GenreForResultDto>> GetAllAsync()
        {
            var genres = await this.unitOfWork.Genres
                .GetAll()
                .Include(x => x.ParentGenre)
                .ToListAsync();

            return this.mapper.Map<IEnumerable<GenreForResultDto>>(genres);
        }

        public async Task<IEnumerable<GenreForResultDto>> GetByGameKeyAsync(string gameKey)
        {
            var genres = await this.unitOfWork.Genres.GetByGameKeyAsync(gameKey);
            return this.mapper.Map<IEnumerable<GenreForResultDto>>(genres);
        }

        public async Task<GenreForResultDto?> GetByIdAsync(Guid id)
        {
            var genre = await this.unitOfWork.Genres.FindByIdAsync(id);
            return this.mapper.Map<GenreForResultDto>(genre);
        }

        /// </inheritdoc/>
        public async Task<GenreForUpdateDto?>  GenreForUpdateDtoAsync(Guid id)
        {
            var genre = await this.unitOfWork.Genres.FindByIdAsync(id);
            if (genre == null)
            {
                return null; // Genre not found
            }
            return this.mapper.Map<GenreForUpdateDto>(genre);
            
        }
        public async Task<IEnumerable<GenreForResultDto>> GetByParentIdAsync(Guid parentId)
        {
            var genres = await this.unitOfWork.Genres.GetByParentIdAsync(parentId);
            return this.mapper.Map<IEnumerable<GenreForResultDto>>(genres);
        }

        public async Task<bool> UpdateAsync(GenreForUpdateDto dto)
        {
            var existingGenre = await this.unitOfWork.Genres.FindByIdAsync((dto.Id));
            if (existingGenre == null)
            {
                return false; // Genre not found
            }

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
    }
}
