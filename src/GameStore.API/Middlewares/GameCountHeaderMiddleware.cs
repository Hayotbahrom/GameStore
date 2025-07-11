using GameStore.Application.Interfaces;

namespace GameStore.API.Middlewares;

public class GameCountHeaderMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IGameService gameService)
    {
        context.Response.OnStarting(async () =>
        {
            if (context.Response.ContentType?.Contains("application/json") == true)
            {
                var totalGames = await gameService.GetTotalGamesCountAsync();
                context.Response.Headers["X-Total-Numbers-Of-Games"] = totalGames.ToString();
            }
        });

        await _next(context);
    }
}
