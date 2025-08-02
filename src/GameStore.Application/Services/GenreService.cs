// <copyright file="GenreService.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.DTOs.Genres;
using GameStore.Application.Exceptions;
using GameStore.Application.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.Application.Services;

/// <summary>
/// Service for managing genre-related operations.
/// </summary>
public class GenreService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<GenreService> logger) : IGenreService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<GenreService> _logger = logger;

    /// <inheritdoc/>
    public async Task<GenreResultDto> AddAsync(GenreCreationDto dto)
    {
        _logger.LogInformation("AddAsync called. Creating genre with name: {Name}", dto.Name);

        var isExistParentGenre = await ValidateCreationDtoAsync(dto);
        if (isExistParentGenre)
        {
            var genre = new Genre
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                ParentGenreId = dto.ParentGenreId,
            };

            await _unitOfWork.Genres.AddAsync(genre);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation("Genre added successfully with id: {Id}", genre.Id);

            return _mapper.Map<GenreResultDto>(genre);
        }
        else
        {
            _logger.LogWarning("Genre create failed: parent genre id is invalid");
            throw new CustomException(404, "Parent genre not found");
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("DeleteAsync called, Id: {Id}", id);

        var existingGenre = await ValidateGenreAsync(id);

        _unitOfWork.Genres.Delete(existingGenre);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Genre deleted successfully with id: {Id}", id);

        return true;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResultDto>> GetAllAsync()
    {
        _logger.LogInformation("GetAllAsync called.");

        var genres = await _unitOfWork.Genres
            .GetAll()
            .Include(x => x.ParentGenre)
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} genres.", genres.Count);

        return _mapper.Map<IEnumerable<GenreResultDto>>(genres);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetGamesByGenreIdAsync(Guid genreId)
    {
        _logger.LogInformation("GetGamesByGenreIdAsync called with genreId: {GenreId}", genreId);

        var games = await _unitOfWork.Games.GetGamesByGenreIdAsync(genreId);

        _logger.LogInformation("Found {Count} games for genreId: {GenreId}", games.Count(), genreId);

        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<GenreResultDto?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("GetByIdAsync called with id: {Id}", id);

        var genre = await ValidateGenreAsync(id);

        _logger.LogInformation("Genre found. Id: {Id}, Name: {Name}", genre.Id, genre.Name);

        return _mapper.Map<GenreResultDto>(genre);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResultDto>> GetByParentIdAsync(Guid parentId)
    {
        _logger.LogInformation("GetByParentIdAsync called with parentId: {Id}", parentId);

        var genres = await _unitOfWork.Genres.GetByParentIdAsync(parentId);

        _logger.LogInformation("Found {Count} genres under ParentGenreId: {Id}", genres.Count(), parentId);

        return _mapper.Map<IEnumerable<GenreResultDto>>(genres);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(GenreUpdateDto dto)
    {
        _logger.LogInformation("UpdateAsync called. GenreId :{Id}", dto.Id);
        var existingGenre = await ValidateGenreAsync(dto.Id);

        existingGenre.Name = dto.Name;
        existingGenre.ParentGenreId = dto.ParentGenreId;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Genre updated successfuly with id: {Id}", dto.Id);

        return true;
    }

    /// <summary>
    /// Validate genre with given id.
    /// </summary>
    /// <param name="genreId">Genre id.</param>
    /// <returns>Genre model.</returns>
    private async Task<Genre> ValidateGenreAsync(Guid genreId)
    {
        _logger.LogDebug("Validating existence genre. GenreId : {Id} ...", genreId);

        var genre = await _unitOfWork.Genres.FindByIdAsync(genreId);

        if (genre is null)
        {
            _logger.LogWarning("Validation failed, Genre not found, id : {Id}", genreId);
            throw new CustomException(404, $"Genre with ID {genreId} does not exist.");
        }

        return genre;
    }

    /// <summary>
    /// Validate parent genre is exist or not.
    /// </summary>
    /// <param name="dto">Creation dto.</param>
    /// <returns>bool.</returns>
    private async Task<bool> ValidateCreationDtoAsync(GenreCreationDto dto)
    {
        if (dto.ParentGenreId != null)
        {
            _logger.LogDebug("Validating ParentGenreId: {ParentId}", dto.ParentGenreId);

            var parentGenre = await _unitOfWork.Genres.FindByIdAsync((Guid)dto.ParentGenreId);

            if (parentGenre is null)
            {
                _logger.LogWarning("Parent genre not found. ParentGenreId : {ParentId}", dto.ParentGenreId);
                throw new CustomException(404, "Parent genre not found");
            }
        }

        return true;
    }
}
