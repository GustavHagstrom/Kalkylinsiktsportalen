using System.ComponentModel.DataAnnotations;

namespace Core.Models.Abstractions;
public interface ITenantEntety
{
    [StringLength(50)]
    public string TenantId { get; set; }
}
