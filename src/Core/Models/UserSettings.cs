using Core.Models.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Core.Models;
public class UserSettings : IUserEntety
{
    [StringLength(50)]
    [Key]
    public required string UserId { get; set; }
    public bool IsDarkMode { get; set; } = false;
}
