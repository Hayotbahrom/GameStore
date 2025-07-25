// <copyright file="PlatformService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using AutoMapper;
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
        var platform = await ValidatePlatformExistAsync(id);

        _unitOfWork.Platforms.Delete(platform);

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Platform deleted with id: {Id}", id);

        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlatformResultDto>> GetAllAsync()
    {
        var platforms = await _unitOfWork.Platforms.GetAll().ToListAsync();

        _logger.LogInformation("GetAllAsync called.");

        return _mapper.Map<IEnumerable<PlatformResultDto>>(platforms);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlatformResultDto>> GetByGameKeyAsync(string gameKey)
    {
        var platforms = await _unitOfWork.Platforms.GetByGameKeyAsync(gameKey);

        _logger.LogInformation("GetByGameKeyAsync called with gamekey: {Gamekey}", gameKey);

        return _mapper.Map<IEnumerable<PlatformResultDto>>(platforms);
    }

    /// <inheritdoc />
    public async Task<PlatformResultDto?> GetByIdAsync(Guid id)
    {
        var platform = await ValidatePlatformExistAsync(id);

        _logger.LogInformation("GetByIdAsync called with id: {Id}", id);

        return _mapper.Map<PlatformResultDto>(platform);
    }

    /// <inheritdoc />
    public async Task<bool> UpdateAsync(PlatformUpdateDto dto)
    {
        var existingPlatform = await ValidatePlatformExistAsync(dto.Id);

        existingPlatform.Type = dto.Type;

        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Platform updated with id: {Id}", dto.Id);

        return true;
    }

    private async Task<Platform> ValidatePlatformExistAsync(Guid platformId)
    {
        _logger.LogDebug("Validating platform creation input...");

        var platform = await _unitOfWork.Platforms.FindByIdAsync(platformId);

        return platform ?? throw new CustomException(404, "Platform is not found");
    }
}
