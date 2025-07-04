// <copyright file="GameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Services;

/// <summary>
/// Service for managing game-related operations.
/// </summary>
public class GameService(IUnitOfWork unitOfWork, IMapper mapper) : IGameService
{
    /// <summary>
    /// the unit of work instance used for database operations.
    /// </summary>
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// The mapper for mapping between domain entities and DTOs.
    /// </summary>
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public Task<byte[]> GenerateGameFileAsync(string key)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameForResultDto>> GetAllAsync()
    {
        var games = await _unitOfWork.Games.GetAll().ToListAsync();
        return _mapper.Map<IEnumerable<GameForResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameForResultDto>> GetByGenreAsync(Guid genreId)
    {
        var games = await _unitOfWork.Games.GetByGenreAsync(genreId);
        return _mapper.Map<IEnumerable<GameForResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameForResultDto>> GetByPlatformAsync(Guid platformId)
    {
        var games = await _unitOfWork.Games.GetByPlatformAsync(platformId);
        return _mapper.Map<IEnumerable<GameForResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<GameForResultDto> GetByIdAsync(Guid id)
    {
        var game = await _unitOfWork.Games.GetFullGameByIdAsync(id);
        return _mapper.Map<GameForResultDto>(game);
    }

    /// <inheritdoc/>
    public async Task<GameForResultDto> GetByKeyAsync(string key)
    {
        var game = await _unitOfWork.Games.GetByKeyAsync(key);
        return _mapper.Map<GameForResultDto>(game);
    }

    /// <inheritdoc/>
    public async Task<GameForResultDto> AddGameAsync(GameForCreationDto game)
    {
        ValidateGame(game);

        var resultGame = MapToGameEntity(game);

        await _unitOfWork.Games.AddAsync(resultGame);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GameForResultDto>(resultGame);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteGameAsync(Guid id)
    {
        var game = await _unitOfWork.Games.GetFullGameByIdAsync(id);
        if (game is null)
        {
            return false;
        }

        _unitOfWork.Games.Delete(game);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateGameAsync(GameForUpdateDto game)
    {
        var existingGame = await ValidateAndGetGameAsync(game.Id);

        ApplyGameUpdates(existingGame, game);

        var result = await _unitOfWork.SaveChangesAsync();
        return result > 0;
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

    /// <summary>
    /// Map to game entity given creation dto.
    /// </summary>
    /// <param name="game">Game creation dto.</param>
    /// <returns>Game entity.</returns>
    private static Game MapToGameEntity(GameForCreationDto game)
    {
        return new Game
        {
            Id = Guid.NewGuid(),
            Name = game.Name,
            Description = game.Description,
            Key = string.IsNullOrWhiteSpace(game.Key)
                ? GenerateKeyFromName(game.Name ?? string.Empty)
                : game.Key,
            GameGenres = game.GenreIds.Select(id => new GameGenre { GenreId = id }).ToList(),
            GamePlatforms = game.PlatformIds.Select(id => new GamePlatform { PlatformId = id }).ToList(),
        };
    }

    /// <summary>
    /// Validate game creation dto.
    /// </summary>
    /// <param name="game">Game creation dto.</param>
    private static void ValidateGame(GameForCreationDto game)
    {
        if (game == null)
        {
            throw new ArgumentNullException(nameof(game), "Game cannot be null.");
        }
    }

    /// <summary>
    /// Applying game updated.
    /// </summary>
    /// <param name="existingGame">Game model.</param>
    /// <param name="game">GameForUpdate DTO.</param>
    private static void ApplyGameUpdates(Game existingGame, GameForUpdateDto game)
    {
        existingGame.Name = game.Name;
        existingGame.Description = game.Description;
        existingGame.Key = string.IsNullOrWhiteSpace(game.Key) ? GenerateKeyFromName(game.Name ?? string.Empty) : game.Key;
        existingGame.GameGenres = game.GenreIds
           .Select(id => new GameGenre { GameId = game.Id, GenreId = id })
           .ToList();

        existingGame.GamePlatforms = game.PlatformIds
            .Select(id => new GamePlatform { GameId = game.Id, PlatformId = id })
            .ToList();
    }

    /// <summary>
    /// Validate game with given id.
    /// </summary>
    /// <param name="gameId">Game id.</param>
    /// <returns>Game model.</returns>
    private async Task<Game> ValidateAndGetGameAsync(Guid gameId)
    {
        var game = await _unitOfWork.Games.GetFullGameByIdAsync(gameId);
        return game ?? throw new ArgumentException($"Game with ID {gameId} does not exist.");
    }
}
