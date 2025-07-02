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

            modelBuilder.Entity<Game>().HasData(
                 new Game
                 {
                     Id = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                     Name = "Super Strategy Game",
                     Key = "super-strategy",
                     Description = "A deep and challenging strategy game.",
                 },
                 new Game
                 {
                     Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                     Name = "Arcade Shooter",
                     Key = "arcade-shooter",
                     Description = "Fast-paced arcade FPS experience.",
                 });

            // Seed Platforms
            modelBuilder.Entity<Platform>().HasData(
                new Platform { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Type = "Mobile" },
                new Platform { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Type = "Browser" },
                new Platform { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Type = "Desktop" },
                new Platform { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Type = "Console" });

            // Seed Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = Guid.Parse("aaa11111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Strategy" },
                new Genre { Id = Guid.Parse("aaa22222-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "RTS", ParentGenreId = Guid.Parse("aaa11111-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                new Genre { Id = Guid.Parse("aaa33333-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "TBS", ParentGenreId = Guid.Parse("aaa11111-aaaa-aaaa-aaaa-aaaaaaaaaaaa") },
                new Genre { Id = Guid.Parse("bbb11111-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "RPG" },
                new Genre { Id = Guid.Parse("ccc11111-cccc-cccc-cccc-cccccccccccc"), Name = "Action" },
                new Genre { Id = Guid.Parse("ccc22222-cccc-cccc-cccc-cccccccccccc"), Name = "FPS", ParentGenreId = Guid.Parse("ccc11111-cccc-cccc-cccc-cccccccccccc") });

            // Seed GameGenre
            modelBuilder.Entity<GameGenre>().HasData(
                new GameGenre
                {
                    GameId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    GenreId = Guid.Parse("aaa11111-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                },
                new GameGenre
                {
                    GameId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    GenreId = Guid.Parse("ccc22222-cccc-cccc-cccc-cccccccccccc"),
                });

            // Seed GamePlatform
            modelBuilder.Entity<GamePlatform>().HasData(
                new GamePlatform
                {
                    GameId = Guid.Parse("99999999-9999-9999-9999-999999999999"),
                    PlatformId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                },
                new GamePlatform
                {
                    GameId = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    PlatformId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                });
        }
    }
}
