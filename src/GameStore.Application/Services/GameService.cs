// <copyright file="GameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.Exceptions;
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
    private static readonly JsonSerializerOptions? _jsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.Preserve,
    };

    /// <inheritdoc/>
    public async Task<byte[]> GenerateGameFileAsync(string key)
    {
        var game = await _unitOfWork.Games.GetByKeyAsync(key);
        if (game == null)
        {
            return null;
        }

        var gameDto = _mapper.Map<GameResultDto>(game);

        var serializedGame = JsonSerializer.Serialize(gameDto, _jsonOptions);

        var fileContent = Encoding.UTF8.GetBytes(serializedGame);

        return fileContent;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetAllAsync()
    {
        var games = await _unitOfWork.Games.GetAll().ToListAsync();
        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetByGenreIdAsync(Guid genreId)
    {
        var games = await _unitOfWork.Games.GetByGenreIdAsync(genreId);
        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetByPlatformIdAsync(Guid platformId)
    {
        var games = await _unitOfWork.Games.GetByPlatformIdAsync(platformId);
        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<GameResultDto> GetByIdAsync(Guid id)
    {
        var game = await ValidateAndGetGameAsync(id);
        return _mapper.Map<GameResultDto>(game);
    }

    /// <inheritdoc/>
    public async Task<GameResultDto> GetByKeyAsync(string key)
    {
        var game = await _unitOfWork.Games.GetByKeyAsync(key);
        return _mapper.Map<GameResultDto>(game) ?? throw new CustomException(404, "Game is not found");
    }

    /// <inheritdoc/>
    public async Task<GameResultDto> AddAsync(GameCreationDto game)
    {
        ValidateGame(game);

        var resultGame = MapToGameEntity(game);

        await _unitOfWork.Games.AddAsync(resultGame);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<GameResultDto>(resultGame);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var game = await ValidateAndGetGameAsync(id);
        if (game is null)
        {
            return false;
        }

        _unitOfWork.Games.Delete(game);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(GameUpdateDto game)
    {
        var existingGame = await ValidateAndGetGameAsync(game.Id);

        ApplyGameUpdates(existingGame, game);

        await _unitOfWork.SaveChangesAsync();
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

    /// <summary>
    /// Map to game entity given creation dto.
    /// </summary>
    /// <param name="game">Game creation dto.</param>
    /// <returns>Game entity.</returns>
    private static Game MapToGameEntity(GameCreationDto game)
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
    private static void ValidateGame(GameCreationDto game)
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
    private static void ApplyGameUpdates(Game existingGame, GameUpdateDto game)
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
        return game ?? throw new CustomException(404, "Game is not found.");
    }
}
