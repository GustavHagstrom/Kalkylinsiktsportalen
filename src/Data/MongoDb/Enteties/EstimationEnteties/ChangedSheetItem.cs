using System.ComponentModel.DataAnnotations;

namespace Data.MongoDb.Enteties.EstimationEnteties;
public class SheetItemChange
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string UnitOfChangeId { get; set; } = string.Empty;
    public UnitOfChange? UnitOfChange { get; set; }
    [StringLength(50)]
    public string SheetItemId { get; set; } = string.Empty;
    public SheetItem? SheetItem { get; set; }
    public double? Quantity { get; set; }
}
