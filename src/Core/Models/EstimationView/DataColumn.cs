using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public class DataColumn : IViewEntity
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Order { get; set; }
    public int Width { get; set; } = 10;

    [StringLength(50)]
    public string DataSectionId { get; set; } = string.Empty;
    public DataSection? DataSection { get; set; }
}
