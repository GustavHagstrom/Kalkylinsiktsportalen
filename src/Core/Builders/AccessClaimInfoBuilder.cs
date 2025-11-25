using Core.Abstractions;
using Core.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Builders;
public class AccessClaimInfoBuilder(IServiceCollection Services)
{
    private readonly HashSet<Type> _accessValuesTypes = new();

    public AccessClaimInfoBuilder AddAccessClaimInfo<T>() where T : class, IAccessClaimInfo
    {
        if (_accessValuesTypes.Contains(typeof(T)))
        {
            throw new InvalidOperationException($"AccessClaimInfo {typeof(T).Name} already exists");
        }
        _accessValuesTypes.Add(typeof(T));
        Services.AddAccessClaimInfo<T>();
        return this;
    }
}
