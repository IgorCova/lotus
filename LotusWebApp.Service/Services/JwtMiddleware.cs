using LotusWebApp.Data;

namespace LotusWebApp.Services;

public class JwtMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, TokenService tokenService, ApplicationDbContext dbContext)
    {
        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();
        var userId = tokenService.ValidateToken(token);
        if (userId != null)
        {
            // attach user to context on successful jwt validation
            context.Items["User"] = await dbContext.Users.FindAsync(userId);
        }

        await next(context);
    }
}