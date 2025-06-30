// <copyright file="MappingProfile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
namespace GameStore.Application.Mappings
{
    using AutoMapper;
    using GameStore.Application.DTOs.Games;
    using GameStore.Application.DTOs.Genres;
    using GameStore.Application.DTOs.Platforms;
    using GameStore.Domain.Entities;

    /// <summary>
    /// Mapping profile for the Game Store application using AutoMapper.
    /// </summary>
    public class MappingProfile : Profile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MappingProfile"/> class.
        /// </summary>
        public MappingProfile()
        {
            // Game
            this.CreateMap<Game, GameForCreationDto>().ReverseMap();
            this.CreateMap<Game, GameForUpdateDto>().ReverseMap();
            this.CreateMap<Game, GameForResultDto>()
                .ForMember(dest => dest.Genres, 
                opt => opt.MapFrom(src => src.GameGenres.Select(gg => gg.Genre != null ? gg.Genre.Name : string.Empty)))
                .ForMember(dest => dest.Platforms, 
                opt => opt.MapFrom(src => src.GamePlatforms.Select(gp => gp.Platform != null ? gp.Platform.Type : string.Empty)))
                .ReverseMap();

            // Genre
            this.CreateMap<Genre, GenreForCreationDto>().ReverseMap();
            this.CreateMap<Genre, GenreForUpdateDto>().ReverseMap();
            this.CreateMap<Genre, GenreForResultDto>().ReverseMap();

            // Platform
            this.CreateMap<Platform, PlatformForCreationDto>().ReverseMap();
            this.CreateMap<Platform, PlatformForUpdateDto>().ReverseMap();
            this.CreateMap<Platform, PlatformForResultDto>().ReverseMap();
        }
    }
}
