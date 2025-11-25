using System.ComponentModel.DataAnnotations;

namespace Core.Models.Authorization;
public class RoleAccess
{
    [StringLength(50)]
    public string RoleId { get; set; } = string.Empty;
    public Role? Role { get; set; }
    public string AccessClaimValue { get; set; } = string.Empty;
}
