using Core.Models.Authorization;
using Core.Models.EstimationView;

namespace ViewSettingsModule.Data.Abstractions;
internal interface IRoleViewStore
{
    Task<IEnumerable<string>> GetPickedRoleIdsAsync(View view);
    Task<IEnumerable<Role>> GetRolesAsync(string tenantId);
    Task PickAsync(View view, IEnumerable<string> roleIds);
    Task UnpickAsync(View view, IEnumerable<string> roleIds);
}
