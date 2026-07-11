using BlazorServer.AppShell.Authentication;
using BlazorServer.Modules.Access.Authentication;
using BlazorServer.Shared.Services.Authentication;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace BlazorServer.Middlewares;

public class ClaimsLoaderMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User?.Identity is not null && context.User.Identity.IsAuthenticated)
        {
            var userId = context.User.GetUserId();
            var orgId = context.User.GetOrganizationId();
            if (userId is not null && orgId is not null)
            {
                using (var scope = context.RequestServices.CreateScope())
                {
                    var sp = scope.ServiceProvider;
                    var roleService = sp.GetRequiredService<IRoleService>();
                    var claimsTransofmation = sp.GetRequiredService<IClaimsTransformation>();

                    await claimsTransofmation.TransformAsync(context.User);

                    var isAdmin = await roleService.IsAdminAsync(userId, orgId);
                    if (isAdmin)
                    {
                        SetAdminRole(context.User);
                    }
                }
            }
        }
        await _next(context);
    }
    static void SetAdminRole(ClaimsPrincipal principal)
    {
        ((ClaimsIdentity)principal.Identity!).AddClaim(new Claim(ClaimTypes.Role, AccessPolicies.ApplicationAdminRoleClaimValue));
    }
}
public static class ClaimsLoaderMiddlewareExtensions
{
    public static IApplicationBuilder UseClaimsLoader(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ClaimsLoaderMiddleware>();
    }
}
