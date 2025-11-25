
using Core.Abstractions;

namespace SharedModule.Services;

public interface IAccessClaimInfoContainer
{
    IReadOnlyCollection<IAccessClaimInfo> AccessClaimInfos { get; }
}