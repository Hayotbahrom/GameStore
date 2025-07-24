using GameStore.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.API.Middlewares;

public class GameCountHeaderMiddleware(RequestDelegate next, IMemoryCache cache)
{
    private readonly RequestDelegate _next = next;
    private readonly IMemoryCache _cache = cache;

    public async Task InvokeAsync(HttpContext context, IGameService gameService)
    {
        context.Response.OnStarting(async () =>
        {
            if (context.Response.ContentType?.Contains("application/json") == true)
            {
                if (!_cache.TryGetValue("TotalGamesCount", out int totalGames))
                {
                    totalGames = await gameService.GetTotalGamesCountAsync();
                    _cache.Set("TotalGamesCount", totalGames, TimeSpan.FromMinutes(1));
                }

                context.Response.Headers["X-Total-Numbers-Of-Games"] = totalGames.ToString();
            }
        });

        await _next(context);
    }
}
