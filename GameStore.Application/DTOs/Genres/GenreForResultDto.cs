// <copyright file="GenreForResultDto.cs" company="Epam">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.DTOs.Genres
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Represents of genre for result DTO.
    /// </summary>
    public class GenreForResultDto
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
        public string? ParentGenreName { get; set; }
    }
}
