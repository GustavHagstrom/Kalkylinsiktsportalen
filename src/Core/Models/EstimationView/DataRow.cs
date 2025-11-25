using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public class DataRow : IViewEntity
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string DataSectionId { get; set; } = string.Empty;
    public DataSection? DataSection { get; set; }
    public int Order { get; set; }
    public int Height { get; set; } = 10;
}
