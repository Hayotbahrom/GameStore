// <copyright file="PlatformService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Exceptions;
using GameStore.Application.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GameStore.Application.Services;

/// <summary>
/// Service for managing platform-related operations.
/// </summary>
public class PlatformService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PlatformService> logger) : IPlatformService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;
    private readonly ILogger<PlatformService> _logger = logger;

    /// <inheritdoc />
    public async Task<PlatformResultDto> CreateAsync(PlatformCreationDto dto)
    {
        _logger.LogInformation("CreateAsync called. Creating platform with type : {Type}", dto.Type);

        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Type = dto.Type,
        };

        await _unitOfWork.Platforms.AddAsync(platform);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Platform added successfully with id: {Id}", platform.Id);

        return _mapper.Map<PlatformResultDto>(platform);
    }

    /// <inheritdoc />
    public async Task<bool> DeleteAsync(Guid id)
    {
        _logger.LogInformation("DeleteAsync called for PlatformId: {Id}", id);

        var platform = await ValidatePlatformExistAsync(id);

        _unitOfWork.Platforms.Delete(platform);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Platform deleted with id: {Id}", id);

        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlatformResultDto>> GetAllAsync()
    {
        _logger.LogDebug("GetAllAsync called.");

        var platforms = await _unitOfWork.Platforms.GetAll().ToListAsync();

        _logger.LogInformation("Retrieved {Count} platforms.", platforms.Count);

        return _mapper.Map<IEnumerable<PlatformResultDto>>(platforms);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<GameResultDto>> GetGamesByPlatformIdAsync(Guid platformId)
    {
        _logger.LogInformation("GetGamesByPlatformIdAsync called with genreId: {PlatformId}", platformId);

        var games = await _unitOfWork.Games.GetGamesByPlatformIdAsync(platformId);

        _logger.LogInformation("Found {GameCount} games for PlatformId: {Id}", games.Count(), platformId);

        return _mapper.Map<IEnumerable<GameResultDto>>(games);
    }

    /// <inheritdoc />
    public async Task<PlatformResultDto?> GetByIdAsync(Guid id)
    {
        _logger.LogDebug("GetByIdAsync called with id: {Id}", id);

        var platform = await ValidatePlatformExistAsync(id);

        _logger.LogInformation("Platform found. Id: {PlatformId}, Type: {Type}", platform.Id, platform.Type);

        return _mapper.Map<PlatformResultDto>(platform);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(PlatformUpdateDto dto)
    {
        _logger.LogInformation("UpdateAsync called. PlatformId: {Id}, new type: {Type}", dto.Id, dto.Type);

        var existingPlatform = await ValidatePlatformExistAsync(dto.Id);

        existingPlatform.Type = dto.Type;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Platform updated successfully. Id: {Id}", dto.Id);

        return true;
    }

    private async Task<Platform> ValidatePlatformExistAsync(Guid platformId)
    {
        _logger.LogDebug("Validating platform creation input...");

        var platform = await _unitOfWork.Platforms.FindByIdAsync(platformId);

        if (platform == null)
        {
            _logger.LogWarning("Platform not found. id: {Id}", platformId);
            throw new CustomException(404, "Platform is not found");
        }

        return platform;
    }
}
