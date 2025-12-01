using BlazorServer.Shared.Abstractions;

namespace BlazorServer.Shared.Services.Authentication;

public interface IAccessClaimInfoContainer
{
    IReadOnlyCollection<IAccessClaimInfo> AccessClaimInfos { get; }
}