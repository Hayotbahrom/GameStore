// <copyright file="GamePlatform.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Domain.Entities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents the relationship between a game and its platform in the system.
    /// </summary>
    public class GamePlatform
    {
        /// <summary>
        /// Gets or sets the Id of the associated game.
        /// </summary>
        public Guid GameId { get; set; }

        /// <summary>
        /// Gets or sets the game associated.
        /// </summary>
        public Game? Game { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the platform.
        /// </summary>
        public Guid PlatformId { get; set; }

        /// <summary>
        /// Gets or sets the platform associated.
        /// </summary>
        public Platform? Platform { get; set; }
    }
}
