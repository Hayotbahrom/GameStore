// <copyright file="GameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Application.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using GameStore.Application.DTOs.Games;
    using GameStore.Application.Interfaces;
    using GameStore.Domain.Entities;
    using GameStore.Infrastructure.IRepositories;
    using GameStore.Infrastructure.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Service for managing game-related operations.
    /// </summary>
    public class GameService : IGameService
    {
        /// <summary>
        /// the unit of work instance used for database operations.
        /// </summary>
        private readonly IUnitOfWork unitOfWork;

        /// <summary>
        /// The mapper for mapping between domain entities and DTOs.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="GameService"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work instance. </param>
        /// <param name="mapper">The mapper instance for mapping between entities and DTOs.</param>
        public GameService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        /// <inheritdoc/>
        public async Task<GameForResultDto> AddGameAsync(GameForCreationDto game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game), "Game cannot be null.");
            }

            var resultGame = new Game
            {
                Id = Guid.NewGuid(),
                Name = game.Name,
                Description = game.Description,
                Key = string.IsNullOrWhiteSpace(game.Key) ? GenerateKeyFromName(game.Name ?? string.Empty) : game.Key,
                GameGenres = game.GenreIds.Select(id => new GameGenre { GenreId = id }).ToList(),
                GamePlatforms = game.PlatformIds.Select(id => new GamePlatform { PlatformId = id }).ToList(),
            };

            await this.unitOfWork.Games.AddAsync(resultGame);
            await this.unitOfWork.SaveChangesAsync();
            return this.mapper.Map<GameForResultDto>(resultGame);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteGameAsync(Guid id)
        {
            var game = await this.unitOfWork.Games.GetFullGameByIdAsync(id);
            if (game is null)
            {
                return false;
            }

            this.unitOfWork.Games.Delete(game);
            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public Task<byte[]> GenerateGameFileAsync(string key)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GameForResultDto>> GetAllAsync()
        {
            var games = await this.unitOfWork.Games.GetAll().ToListAsync();
            return this.mapper.Map<IEnumerable<GameForResultDto>>(games);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GameForResultDto>> GetByGenreAsync(Guid genreId)
        {
            var games = await this.unitOfWork.Games.GetByGenreAsync(genreId);
            return this.mapper.Map<IEnumerable<GameForResultDto>>(games);
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<GameForResultDto>> GetByPlatformAsync(Guid platformId)
        {
            var games = await this.unitOfWork.Games.GetByPlatformAsync(platformId);
            return this.mapper.Map<IEnumerable<GameForResultDto>>(games);
        }

        /// <inheritdoc/>
        public async Task<GameForResultDto> GetByIdAsync(Guid id)
        {
            var game = await this.unitOfWork.Games.GetFullGameByIdAsync(id);
            return this.mapper.Map<GameForResultDto>(game);
        }

        /// <inheritdoc/>
        public async Task<GameForResultDto> GetByKeyAsync(string key)
        {
            var game = await this.unitOfWork.Games.GetByKeyAsync(key);
            return this.mapper.Map<GameForResultDto>(game);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateGameAsync(GameForUpdateDto game)
        {
            var existingGame = await this.unitOfWork.Games.GetFullGameByIdAsync(game.Id);
            if (existingGame == null)
            {
                throw new ArgumentException($"Game with ID {game.Id} does not exist.", nameof(game));
            }

            existingGame.Name = game.Name;
            existingGame.Description = game.Description;
            existingGame.Key = string.IsNullOrWhiteSpace(game.Key) ? GenerateKeyFromName(game.Name ?? string.Empty) : game.Key;
            existingGame.GameGenres = game.GenreIds.Select(id => new GameGenre { GenreId = id }).ToList();
            existingGame.GamePlatforms = game.PlatformIds.Select(id => new GamePlatform { PlatformId = id }).ToList();

            await this.unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Generate a unique key for a game based on its name.
        /// </summary>
        /// <param name="name">Game name.</param>
        /// <returns>A Unique game key.</returns>
        private static string GenerateKeyFromName(string name)
        {
            return name.Trim().Replace(" ", "_").ToLowerInvariant();
        }
    }
}
