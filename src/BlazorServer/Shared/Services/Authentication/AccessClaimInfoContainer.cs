using BlazorServer.Shared.Abstractions;

namespace BlazorServer.Shared.Services.Authentication;
public class AccessClaimInfoContainer : IAccessClaimInfoContainer
{
    public AccessClaimInfoContainer(IEnumerable<IAccessClaimInfo> accessClaimInfos)
    {
        AccessClaimInfos = accessClaimInfos.ToHashSet();
    }
    public IReadOnlyCollection<IAccessClaimInfo> AccessClaimInfos { get; }

}
