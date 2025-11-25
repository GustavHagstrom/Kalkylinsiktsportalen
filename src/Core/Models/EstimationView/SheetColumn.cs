using Core.Enums.ViewTemplate;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;

public class SheetColumn : IViewEntity
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Order { get; set; }
    public int Width { get; set; } = 10;
    public bool IsVisible { get; set; } = true;
    public SheetColumnType ColumnType { get; set; }

    [StringLength(50)]
    public string SheetSectionId { get; set; } = string.Empty;
    public SheetSection? SheetSection { get; set; }
    

}
