// <copyright file="Platform.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a platform on which games can be played.
    /// </summary>
    public class Platform
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

        /// <summary>
        /// Gets or sets the list of game-platform relationships.
        /// </summary>
        public ICollection<GamePlatform>? GamePlatforms { get; set; }
    }
}
