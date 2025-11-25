using Core.Enums.BidconAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.MongoDb.Enteties.EstimationEnteties;

public class SheetItem
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string EstimationId { get; set; } = string.Empty;
    public Estimation? Estimation { get; set; }
    public string Description { get; set; } = string.Empty;
    [StringLength(50)]
    public string? ParentId { get; set; }
    public SheetItem? Parent { get; set; }
    public int Position { get; set; }
    public int RowType { get; set; }
    public double? Quantity { get; set; }
    public List<SheetItem> Children { get; set; } = [];
    public string Remark { get; set; } = string.Empty;
    public string? Unit { get; set; }

    /// <summary>
    /// Is null for all RowTypes but LayerdItems
    /// </summary>
    public double? CostBearerUnitCost { get; set; }
    /// <summary>
    /// Is null for all RowTypes but LayerdItems
    /// </summary>
    public double? CostBearerUnitAskingPrice { get; set; }
}