// <copyright file="GameGenre.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents relationship between a game and its genre in the system.
    /// </summary>
    public class GameGenre
    {
        /// <summary>
        /// Gets or sets the Id of the associated game.
        /// </summary>
        [Required]
        public Guid GameId { get; set; }

        /// <summary>
        /// Gets or sets the game associated.
        /// </summary>
        public Game? Game { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the game.
        /// </summary>
        [Required]
        public Guid GenreId { get; set; }

        /// <summary>
        /// Gets or sets the genre associated.
        /// </summary>
        public Genre? Genre { get; set; }
    }
}
