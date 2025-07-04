﻿using GameStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Application.DTOs.Genres
{
    public class GenreForCreationDto
    {
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
}
