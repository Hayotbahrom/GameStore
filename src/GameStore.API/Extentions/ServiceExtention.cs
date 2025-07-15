using GameStore.Application.Interfaces;
using GameStore.Application.Mappings;
using GameStore.Application.Services;
using GameStore.Infrastructure.IRepositories;
using GameStore.Infrastructure.Repositories;

namespace GameStore.API.Extentions;

public static class ServiceExtention
{
    public static void AddCustomService(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IGenreRepository, GenreRepository>();
        services.AddScoped<IPlatformRepository, PlatformRepository>();
        services.AddScoped<IGameRepository, GameRepository>();

        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IGenreService, GenreService>();
        services.AddScoped<IPlatformService, PlatformService>();
    }
}
