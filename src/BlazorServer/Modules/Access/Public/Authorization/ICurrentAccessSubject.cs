namespace BlazorServer.Modules.Access.Public.Authorization;

public interface ICurrentAccessSubject
{
    Task<AccessSubject?> GetAsync(CancellationToken cancellationToken = default);
}
