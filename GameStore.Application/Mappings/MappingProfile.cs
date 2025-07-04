// <copyright file="MappingProfile.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
using AutoMapper;
using GameStore.Application.DTOs.Games;
using GameStore.Application.DTOs.Genres;
using GameStore.Application.DTOs.Platforms;
using GameStore.Domain.Entities;

namespace GameStore.Application.Mappings;

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
        CreateMap<Game, GameForCreationDto>().ReverseMap();
        CreateMap<Game, GameForUpdateDto>().ReverseMap();
        CreateMap<Game, GameForResultDto>()
            .ForMember(
                dest => dest.Genres,
                opt => opt.MapFrom(src => src.GameGenres.Select(gg => gg.Genre != null ? gg.Genre.Name : string.Empty)))
            .ForMember(
                dest => dest.Platforms,
                opt => opt.MapFrom(src => src.GamePlatforms.Select(gp => gp.Platform != null ? gp.Platform.Type : string.Empty)))
            .ReverseMap();

        // Genre
        CreateMap<Genre, GenreForCreationDto>().ReverseMap();
        CreateMap<Genre, GenreForUpdateDto>().ReverseMap();
        CreateMap<Genre, GenreForResultDto>()
            .ForMember(
            dest => dest.ParentGenreName,
            opt => opt.MapFrom(src => src.ParentGenre != null ? src.ParentGenre.Name : null));

        // Platform
        CreateMap<Platform, PlatformForCreationDto>().ReverseMap();
        CreateMap<Platform, PlatformForUpdateDto>().ReverseMap();
        CreateMap<Platform, PlatformForResultDto>().ReverseMap();
    }
}
