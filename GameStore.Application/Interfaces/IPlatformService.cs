// <copyright file="IPlatformService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using GameStore.Application.DTOs.Platforms;

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
        /// <param name="gameKey"></param>
        /// <returns>List of platforms</returns>
        Task<IEnumerable<PlatformForResultDto>> GetByGameKeyAsync(string gameKey);

        /// <summary>
        /// Creates a new platform.
        /// </summary>
        /// <param name="platform">The platform to create.</param>
        /// <returns>The created platform.</returns>
        Task<PlatformForResultDto> CreatePlatformAsync(PlatformForCreationDto platform);

        /// <summary>
        /// Updates an existing platform.
        /// </summary>
        /// <param name="platform">The platform to update.</param>
        Task<bool> UpdatePlatformAsync(PlatformForUpdateDto platform);

        /// <summary>
        /// Deletes a platform by its ID.
        /// </summary>
        /// <param name="id">The unique identifier of the platform to delete.</param>
        Task<bool> DeletePlatformAsync(Guid id);
    }
}
