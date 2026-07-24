using System.Security.Claims;

namespace BlazorServer.Modules.Access.Public.Authorization;

public interface IApplicationAdminPrincipalEvaluator
{
    bool IsApplicationAdmin(ClaimsPrincipal principal);
}
