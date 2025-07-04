// <copyright file="PlatformService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using AutoMapper;
using GameStore.Application.DTOs.Platforms;
using GameStore.Application.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Application.Services;

/// <summary>
/// Service for managing platform-related operations.
/// </summary>
public class PlatformService(IUnitOfWork unitOfWork, IMapper mapper) : IPlatformService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IMapper _mapper = mapper;

    /// <inheritdoc />
    public async Task<PlatformForResultDto> CreatePlatformAsync(PlatformForCreationDto dto)
    {
        var platform = new Platform
        {
            Id = Guid.NewGuid(),
            Type = dto.Type,
        };

        await _unitOfWork.Platforms.AddAsync(platform);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<PlatformForResultDto>(platform);
    }

    /// <inheritdoc />
    public async Task<bool> DeletePlatformAsync(Guid id)
    {
        var platform = await _unitOfWork.Platforms.FindByIdAsync(id);
        if (platform == null)
        {
            return false;
        }

        _unitOfWork.Platforms.Delete(platform);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlatformForResultDto>> GetAllPlatformsAsync()
    {
        var platforms = await _unitOfWork.Platforms.GetAll().ToListAsync();
        return _mapper.Map<IEnumerable<PlatformForResultDto>>(platforms);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<PlatformForResultDto>> GetByGameKeyAsync(string gameKey)
    {
        var platforms = await _unitOfWork.Platforms.GetByGameKeyAsync(gameKey);
        return _mapper.Map<IEnumerable<PlatformForResultDto>>(platforms);
    }

    /// <inheritdoc />
    public async Task<PlatformForResultDto?> GetPlatformByIdAsync(Guid id)
    {
        var platform = await _unitOfWork.Platforms.FindByIdAsync(id);
        return platform == null ? null : _mapper.Map<PlatformForResultDto>(platform);
    }

    /// <inheritdoc />
    public async Task<bool> UpdatePlatformAsync(PlatformForUpdateDto dto)
    {
        var existingPlatform = await _unitOfWork.Platforms.FindByIdAsync(dto.Id);
        if (existingPlatform == null)
        {
            return false;
        }

        existingPlatform.Type = dto.Type;
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Guid>> GetPlatformIdsByGameNamesAsync(IEnumerable<string> gameNames)
    {
        var platforms = await _unitOfWork.Platforms.GetAll().ToListAsync();
        return platforms
            .Where(p => gameNames.Contains(p.Type))
            .Select(p => p.Id);
    }
}
