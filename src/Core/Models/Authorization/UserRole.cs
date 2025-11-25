using Core.Models.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.Authorization;
public class UserRole : IUserEntety
{
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;
    [StringLength(50)]
    public string RoleId { get; set; } = string.Empty;
    public Role? Role { get; set; }

}
