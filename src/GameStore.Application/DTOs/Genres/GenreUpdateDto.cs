// <copyright file="GenreUpdateDto.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.ComponentModel.DataAnnotations;

namespace GameStore.Application.DTOs.Genres;

/// <summary>
/// Represents of genre for update DTO.
/// </summary>
public class GenreUpdateDto
{
    /// <summary>
    /// Gets or sets the unique identifier for the genre.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the genre.
    /// </summary>
    [Required]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the Id of the parent genre, if this genre is a sub-genre.
    /// </summary>
    public Guid? ParentGenreId { get; set; }
}
