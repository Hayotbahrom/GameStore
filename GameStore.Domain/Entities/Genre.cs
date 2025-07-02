// <copyright file="Genre.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Domain.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents a genre of a game in the system.
    /// </summary>
    public class Genre
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

        /// <summary>
        /// Gets or sets the parent genre.
        /// </summary>
        public Genre? ParentGenre { get; set; }

        /// <summary>
        /// Gets or sets the collection of child genres that belong to this genre, if it is a parent genre.
        /// </summary>
        public ICollection<Genre>? SubGenres { get; set; }

        /// <summary>
        /// Gets or sets the list of game-genre relationships.
        /// </summary>
        public ICollection<GameGenre>? GameGenres { get; set; }
    }
}
