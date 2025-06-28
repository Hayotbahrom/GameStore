// <copyright file="GameForCreationDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace GameStore.Application.DTOs
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents game for creation Data Transfer Object (DTO).
    /// </summary>
    public class GameForCreationDto
    {
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
        /// Gets or sets the list of genre IDs associated with the game.
        /// </summary>
        public List<Guid> GenreIds { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of platform IDs associated with the game.
        /// </summary>
        public List<Guid> PlatformIds { get; set; } = new();
    }
}
