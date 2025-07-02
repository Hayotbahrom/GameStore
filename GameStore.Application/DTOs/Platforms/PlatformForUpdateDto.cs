// <copyright file="PlatformForUpdateDto.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.DTOs.Platforms
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents of platform for update DTO.
    /// </summary>
    public class PlatformForUpdateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the platform.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the type of platform.
        /// </summary>
        [Required]
        public string? Type { get; set; }
    }
}
