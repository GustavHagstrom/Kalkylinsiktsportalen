using Core.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Authorization;
public class Role : ITenantEntety
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string TenantId { get; set; } = string.Empty;
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;
    public List<RoleAccess> RoleAccesses { get; set; } = new();
}
