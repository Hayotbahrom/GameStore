// <copyright file="PlatformForCreationDto.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.DTOs.Platforms
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents of platform for creation DTO.
    /// </summary>
    public class PlatformForCreationDto
    {
        /// <summary>
        /// Gets or sets the type of platform.
        /// </summary>
        [Required]
        public string? Type { get; set; }
    }
}
