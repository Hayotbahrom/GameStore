// <copyright file="PlatformForUpdateDto.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs.Platforms;

/// <summary>
/// Represents of platform for update DTO.
/// </summary>
public class PlatformUpdateDto
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
