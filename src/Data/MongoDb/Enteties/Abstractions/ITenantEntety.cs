using System.ComponentModel.DataAnnotations;

namespace Data.MongoDb.Enteties.Abstractions;
public interface ITenantEntety
{
    [StringLength(50)]
    public string TenantId { get; set; }
}
