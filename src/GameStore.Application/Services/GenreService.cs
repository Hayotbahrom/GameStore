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

namespace GameStore.Application.Services;

/// <summary>
/// Service for managing genre-related operations.
/// </summary>
public class GenreService(IUnitOfWork unitOfWork, IMapper mapper) : IGenreService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetGamesByGenreIdAsync(Guid genreId)
    {
        var games = await _unitOfWork.Games.GetGamesByGenreIdAsync(genreId);
        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc/>
    public async Task<Guid> AddAsync(GenreCreationDto dto)
    {
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
            return genre.Id;
        }
        else
        {
            throw new CustomException(404, "Parent genre not found");
        }
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existingGenre = await ValidateGenreAsync(id);

        _unitOfWork.Genres.Delete(existingGenre);
        var result = await _unitOfWork.SaveChangesAsync();
        return result > 0;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResultDto>> GetAllAsync()
    {
        var genres = await _unitOfWork.Genres
            .GetAll()
            .Include(x => x.ParentGenre)
            .ToListAsync();

        return _mapper.Map<IEnumerable<GenreResultDto>>(genres);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResultDto>> GetByGameKeyAsync(string gameKey)
    {
        var genres = await _unitOfWork.Genres.GetByGameKeyAsync(gameKey);
        return _mapper.Map<IEnumerable<GenreResultDto>>(genres);
    }

    /// <inheritdoc/>
    public async Task<GenreResultDto?> GetByIdAsync(Guid id)
    {
        var genre = await ValidateGenreAsync(id);
        return _mapper.Map<GenreResultDto>(genre);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GenreResultDto>> GetByParentIdAsync(Guid parentId)
    {
        var genres = await _unitOfWork.Genres.GetByParentIdAsync(parentId);
        return _mapper.Map<IEnumerable<GenreResultDto>>(genres);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateAsync(GenreUpdateDto dto)
    {
        var existingGenre = await ValidateGenreAsync(dto.Id);

        existingGenre.Name = dto.Name;
        existingGenre.ParentGenreId = dto.ParentGenreId;

        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Validate genre with given id.
    /// </summary>
    /// <param name="genreId">Genre id.</param>
    /// <returns>Genre model.</returns>
    private async Task<Genre> ValidateGenreAsync(Guid genreId)
    {
        var genre = await _unitOfWork.Genres.FindByIdAsync(genreId);
        return genre ?? throw new CustomException(404, $"Genre with ID {genreId} does not exist.");
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
            var parentGenre = await _unitOfWork.Genres.FindByIdAsync((Guid)dto.ParentGenreId);
            return parentGenre == null ? throw new CustomException(404, "Parent genre not found") : true;
        }
        else
        {
            return true;
        }
    }
}
