// <copyright file="IPlatformService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using GameStore.Application.DTOs.Platforms;

namespace GameStore.Application.Interfaces;

/// <summary>
/// Provides an interface for platform services.
/// </summary>
public interface IPlatformService
{
    /// <summary>
    /// Gets all platforms.
    /// </summary>
    /// <returns>A list of platforms.</returns>
    Task<IEnumerable<PlatformForResultDto>> GetAllPlatformsAsync();

    /// <summary>
    /// Gets a platform by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the platform.</param>
    /// <returns>The platform with the specified ID.</returns>
    Task<PlatformForResultDto?> GetPlatformByIdAsync(Guid id);

    /// <summary>
    /// Gets platforms associated with a specific game by its key.
    /// </summary>
    /// <param name="gameKey">Game key.</param>
    /// <returns>List of platforms.</returns>
    Task<IEnumerable<PlatformForResultDto>> GetByGameKeyAsync(string gameKey);

    /// <summary>
    /// Creates a new platform.
    /// </summary>
    /// <param name="dto">The platform to create.</param>
    /// <returns>The created platform.</returns>
    Task<PlatformForResultDto> CreatePlatformAsync(PlatformForCreationDto dto);

    /// <summary>
    /// Updates an existing platform.
    /// </summary>
    /// <param name="dto">The platform to update.</param>
    /// <returns>If successully updated, return true, else false.</returns>
    Task<bool> UpdatePlatformAsync(PlatformForUpdateDto dto);

    /// <summary>
    /// Deletes a platform by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the platform to delete.</param>
    /// <returns>If successully deleted, return true, else false.</returns>
    Task<bool> DeletePlatformAsync(Guid id);

    /// <summary>
    /// Gets platform IDs by game names.
    /// </summary>
    /// <param name="gameNames">List of game names.</param>
    /// <returns>List of platform IDs.</returns>
    Task<IEnumerable<Guid>> GetPlatformIdsByGameNamesAsync(IEnumerable<string> gameNames);
}
