// <copyright file="IGenreService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GameStore.Application.DTOs.Genres;

    /// <summary>
    /// Provides an interface for genre services.
    /// </summary>
    public interface IGenreService
    {
        Task<IEnumerable<GenreForResultDto>> GetAllAsync();
        Task<GenreForResultDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<GenreForResultDto>> GetByParentIdAsync(Guid parentId);
        Task<IEnumerable<GenreForResultDto>> GetByGameKeyAsync(string gameKey);
        Task<Guid> AddAsync(GenreForCreationDto dto);
        Task<bool> UpdateAsync(GenreForUpdateDto dto);
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// Gets genre IDs associated with specific game names.
        /// </summary>
        /// <param name="gameNames"></param>
        /// <returns></returns>
        public Task<IEnumerable<Guid>> GetGenreIdsByGameNamesAsync(IEnumerable<string> gameNames);
    }
}
