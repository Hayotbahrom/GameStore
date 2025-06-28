// <copyright file="GameForResultDto.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.DTOs
{
    /// <summary>
    /// Represents a game for result Data Transfer Object (DTO).
    /// </summary>
    public class GameForResultDto
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
        /// Gets or sets the unique key of the game.
        /// </summary>
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the game.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the list of genre names associated with the game.
        /// </summary>
        public List<Guid> Genres { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of platform types associated with the game.
        /// </summary>
        public List<Guid> Platforms { get; set; } = new();
    }
}
