// <copyright file="GameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.DTOs.Genres;
using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Exceptions;
using GameStore.Application.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.Application.Services;

/// <summary>
/// Service for managing game-related operations.
/// </summary>
public class GameService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GameService> logger) : IGameService
{
    /// <summary>
    /// the unit of work instance used for database operations.
    /// </summary>
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// The mapper for mapping between domain entities and DTOs.
    /// </summary>
    private readonly IMapper _mapper = mapper;

    private readonly ILogger<GameService> _logger = logger;

    private static readonly JsonSerializerOptions? _jsonOptions = new()
    {
        WriteIndented = true,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
    };

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetAllAsync()
    {
        _logger.LogInformation("GetAllAsync called");

        var games = await _unitOfWork.Games.GetAll().ToListAsync();

        _logger.LogInformation("Retrieved {Count} games", games.Count);

        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlatformResultDto>> GetPlatformsByGameKeyAsync(string gameKey)
    {
        _logger.LogInformation("GetByGameKeyAsync called with gamekey: {Gamekey}", gameKey);

        var platforms = await _unitOfWork.Platforms.GetByGameKeyAsync(gameKey);

        _logger.LogInformation("Retrieved {Count} platforms given gamekey: {GameKey}", platforms.Count(), gameKey);

        return _mapper.Map<IEnumerable<PlatformResultDto>>(platforms);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<GenreResultDto>> GetGenresByGameKeyAsync(string gameKey)
    {
        _logger.LogInformation("GetGenresByGameKeyAsync called with gamekey: {Gamekey}", gameKey);

        var genres = await _unitOfWork.Genres.GetByGameKeyAsync(gameKey);

        _logger.LogInformation("Retrieved {Count} genres given gamekey: {GameKey}", genres.Count(), gameKey);

        return _mapper.Map<IEnumerable<GenreResultDto>>(genres);
    }

    /// <inheritdoc/>
    public async Task<GameResultDto> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("GetByIdAsync called with Id: {Id}", id);

        var game = await ValidateAndGetGameAsync(id);

        _logger.LogInformation("Game found. game id: {Id}", id);

        return _mapper.Map<GameResultDto>(game);
    }

    /// <inheritdoc/>
    public async Task<GameResultDto> GetByKeyAsync(string key)
    {
        _logger.LogInformation("GetByKeyAsync called with key: {Key}", key);

        var game = await _unitOfWork.Games.GetByKeyAsync(key);

        if (game == null)
        {
            _logger.LogWarning("Game not found given gamekey: {GameKey}", key);
            throw new CustomException(404, "Game is not found");
        }

        return _mapper.Map<GameResultDto>(game);
    }

    /// <inheritdoc/>
    public async Task<GameResultDto> AddAsync(GameCreationDto game)
    {
        _logger.LogInformation("AddAsync called.Creating game name : {Name}", game.Name);

        await ValidateGameAsync(game);

        var resultGame = MapToGameEntity(game);

        var existingGame = await _unitOfWork.Games.GetByKeyAsync(resultGame.Key!);
        if (existingGame != null)
        {
            _logger.LogWarning("Game create failed. given gamekey already exist.");
            throw new CustomException(409, $"Game key '{game.Key}' already exists.");
        }

        await _unitOfWork.Games.AddAsync(resultGame);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game added successfully with this Id: {Id}", resultGame.Id);

        return _mapper.Map<GameResultDto>(resultGame);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(string key)
    {
        _logger.LogInformation("DeleteAsync called. Game key: {Key}", key);

        var game = await _unitOfWork.Games.GetByKeyAsync(key);

        if (game == null)
        {
            _logger.LogWarning("Game delete failed. game key is not found ");
            throw new CustomException(404, "Game not found with given Key.");
        }

        _unitOfWork.Games.Delete(game);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game deleted successfully with key: {Key}", key);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(GameUpdateDto game)
    {
        _logger.LogWarning("UpdateAsync called, game id: {Id}, name: {Name}", game.Id, game.Name);

        var existingGame = await ValidateAndGetGameAsync(game.Id);

        ApplyGameUpdates(existingGame, game);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Game updated successfully with id: {Id}", existingGame.Id);

        return true;
    }

    /// <inheritdoc/>
    public async Task<(byte[] FileBytes, string FileName)> GenerateGameFileAsync(string key)
    {
        _logger.LogInformation("GenerateGameFileAsync called. key: {Key}", key);

        var game = await _unitOfWork.Games.GetByKeyAsync(key);

        if (game is null)
        {
            _logger.LogWarning("Game file generate failed. Key not found");
            throw new CustomException(404, "Game key is not found.");
        }

        var gameDto = _mapper.Map<GameResultDto>(game);

        var serializedGame = JsonSerializer.Serialize(gameDto, _jsonOptions);

        var fileContent = Encoding.UTF8.GetBytes(serializedGame);

        var timestamp = DateTime.UtcNow.ToString("ddMMyyyy-HHmmss");

        var fileName = $"{key}_{timestamp}.txt";

        _logger.LogInformation("Game file generated successfully");

        return (fileContent, fileName);
    }

    /// <inheritdoc/>
    public async Task<int> GetTotalGamesCountAsync()
    {
        _logger.LogInformation("GetTotalGamesCount called.");

        var games = await _unitOfWork.Games.GetAll().ToListAsync();

        _logger.LogInformation("Retrieved {Count} games.", games.Count);

        return games.Count;
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
            GameGenres =
            [
                ..game.GenreIds.Select(id => new GameGenre { GenreId = id })
            ],
            GamePlatforms =
            [
                ..game.PlatformIds.Select(id => new GamePlatform { PlatformId = id })
            ],
        };
    }

    /// <summary>
    /// Validate game creation dto.
    /// </summary>
    /// <param name="game">Game creation dto.</param>
    private async Task ValidateGameAsync(GameCreationDto game)
    {
        _logger.LogDebug("Validating game creation input...");

        if (game == null)
        {
            _logger.LogWarning("Game creation failed: input is null.");
            throw new CustomException(400, "Game cannot be null.");
        }

        if (game.GenreIds == null || game.GenreIds.Count == 0)
        {
            _logger.LogWarning("Game creation failed: missing genres.");
            throw new CustomException(400, "At least one genre must be provided.");
        }

        if (game.PlatformIds == null || game.PlatformIds.Count == 0)
        {
            _logger.LogWarning("game creation failed: missing platforms.");
            throw new CustomException(400, "At least one platform must be provided.");
        }

        var existingGenreIds = await _unitOfWork.Genres.GetExistingIdsAsync(game.GenreIds);
        var missingGenreIds = game.GenreIds.Except(existingGenreIds).ToList();
        if (missingGenreIds.Count > 0)
        {
            _logger.LogWarning("Invalid genre ids: {Ids}", string.Join(", ", missingGenreIds));
            throw new CustomException(400, $"The following genre IDs are invalid: {string.Join(", ", missingGenreIds)}");
        }

        var existingPlatformIds = await _unitOfWork.Platforms.GetExistingIdsAsync(game.PlatformIds);
        var missingPlatformIds = game.PlatformIds.Except(existingPlatformIds).ToList();
        if (missingPlatformIds.Count > 0)
        {
            _logger.LogWarning("Invalid platform ids: {Ids}", string.Join(", ", missingPlatformIds));
            throw new CustomException(400, $"The following platform IDs are invalid: {string.Join(", ", missingPlatformIds)}");
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
        existingGame.GameGenres =
        [
            ..game.GenreIds.Select(id => new GameGenre { GameId = game.Id, GenreId = id })
        ];

        existingGame.GamePlatforms =
        [
            ..game.PlatformIds
            .Select(id => new GamePlatform { GameId = game.Id, PlatformId = id })
        ];
    }

    /// <summary>
    /// Validate game with given id.
    /// </summary>
    /// <param name="gameId">Game id.</param>
    /// <returns>Game model.</returns>
    private async Task<Game> ValidateAndGetGameAsync(Guid gameId)
    {
        _logger.LogDebug("Validating  existance game, game id: {Id}", gameId);

        var game = await _unitOfWork.Games.GetFullGameByIdAsync(gameId);

        if (game is null)
        {
            _logger.LogWarning("Validation failed, game not found.");
            throw new CustomException(404, "Game is not found.");
        }

        return game;
    }
}
