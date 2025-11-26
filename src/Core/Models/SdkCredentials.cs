using Core.Abstractions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Core.Models;
public class SdkCredentials : ITenantEntety
{
    [Key]
    [StringLength(50)]
    public string TenantId { get; set; } = string.Empty;
    [StringLength(50)]
    public string User { get; set; } = string.Empty;
    [StringLength(50)]
    public string Password { get; set; } = string.Empty;
    [StringLength(500)]
    public string ConfigPath { get; set; } = string.Empty;
}
