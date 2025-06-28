// <copyright file="GameForUpdateDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.DTOs
{
    /// <summary>
    /// Represents a game for update Data Transfer Object (DTO).
    /// </summary>
    public class GameForUpdateDto
    {
        /// <summary>
        /// Gets or sets the unique identifier of the game.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the game.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the game.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the unique key of the game.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the associated genre IDs.
        /// </summary>
        public List<Guid> GenreIds { get; set; } = new();

        /// <summary>
        /// Gets or sets the associated platform IDs.
        /// </summary>
        public List<Guid> PlatformIds { get; set; } = new();
    }
}
