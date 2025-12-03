using BlazorServer.Shared.Models;
using System.Collections.ObjectModel;

namespace BlazorServer.Shared.Registration;

public class ClaimInfoCollection
{
    private readonly HashSet<AccessClaimInfo> _claimInfos = new();
    public ReadOnlySet<AccessClaimInfo> AccessClaimInfos => _claimInfos.AsReadOnly();
    public void AddAccessClaimInfo(AccessClaimInfo claimInfo)
    {
        _claimInfos.Add(claimInfo);
    }
}
