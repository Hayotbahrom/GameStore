// <copyright file="Game.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using System.ComponentModel.DataAnnotations;

namespace GameStore.Domain.Entities;

/// <summary>
/// Represents a game in the system.
/// </summary>
public class Game
{
    /// <summary>
    /// Gets or sets the unique identifier for the game.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the game.
    /// </summary>
    [Required]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the unique key of the game.
    /// </summary>
    [Required]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the description of the game.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Gets or sets the list of game-genre relationships.
    /// </summary>
    public ICollection<GameGenre> GameGenres { get; set; }

    /// <summary>
    /// Gets or sets the list of game-platform relationships.
    /// </summary>
    public ICollection<GamePlatform> GamePlatforms { get; set; }
}
