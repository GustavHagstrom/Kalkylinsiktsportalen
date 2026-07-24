using System.Security.Claims;

namespace BlazorServer.Modules.Access.Public.Authorization;

public interface IAccessSubjectFactory
{
    AccessSubject? Create(ClaimsPrincipal principal);
}
