// <copyright file="GameStoreDbContext.cs" company="GameStore">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Infrastructure.DbContexts
{
    using GameStore.Domain.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Represents the database context for the Game Store application.
    /// </summary>
    public class GameStoreDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GameStoreDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets  the collection of games in the database.
        /// </summary>
        public DbSet<Game> Games { get; set; }

        /// <summary>
        /// Gets or sets the collection of genres in the database.
        /// </summary>
        public DbSet<Genre> Genres { get; set; }

        /// <summary>
        /// Gets or sets the collection of platforms in the database.
        /// </summary>
        public DbSet<Platform> Platforms { get; set; }

        /// <summary>
        /// Gets or sets the collection of game-genre relationships in the database.
        /// </summary>
        public DbSet<GameGenre> GameGenres { get; set; }

        /// <summary>
        /// Gets or sets the collection of game-platform relationships in the database.
        /// </summary>
        public DbSet<GamePlatform> GamePlatforms { get; set; }

        /// <summary>
        /// Configure models and relationships using Fluent API.
        /// </summary>
        /// <param name="modelBuilder">the model builder used to configure entities. </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Game
            modelBuilder.Entity<Game>()
                .HasIndex(g => g.Key)
                .IsUnique();

            // Genre parent-child
            modelBuilder.Entity<Genre>()
                .HasOne(g => g.ParentGenre)
                .WithMany(g => g.SubGenres)
                .HasForeignKey(g => g.ParentGenreId)
                .OnDelete(DeleteBehavior.Restrict);

            // Game-Genre relationship
            modelBuilder.Entity<GameGenre>()
                .HasKey(gg => new { gg.GameId, gg.GenreId });

            modelBuilder.Entity<GameGenre>()
                .HasOne(gg => gg.Game)
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GameId);

            modelBuilder.Entity<GameGenre>()
                .HasOne(gg => gg.Genre)
                .WithMany(g => g.GameGenres)
                .HasForeignKey(gg => gg.GenreId);

            // Game-Platform relationship
            modelBuilder.Entity<GamePlatform>()
                .HasKey(gp => new { gp.GameId, gp.PlatformId });

            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Game)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.GameId);

            modelBuilder.Entity<GamePlatform>()
                .HasOne(gp => gp.Platform)
                .WithMany(g => g.GamePlatforms)
                .HasForeignKey(gp => gp.PlatformId);
        }
    }
}
