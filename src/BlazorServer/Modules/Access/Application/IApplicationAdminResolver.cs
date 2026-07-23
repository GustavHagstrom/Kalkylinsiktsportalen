namespace BlazorServer.Modules.Access.Application;

public interface IApplicationAdminResolver
{
    Task<bool> IsApplicationAdminAsync(
        string userId,
        string organizationId,
        CancellationToken cancellationToken = default);
}
