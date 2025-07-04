// <copyright file="PlatformForResultDto.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs.Platforms;

/// <summary>
/// Represents of platform for result DTO.
/// </summary>
public class PlatformForResultDto
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
