using Core.Enums.BidconAccess;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Models.EstimationEnteties;

public class SheetItem
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string EstimationId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SheetItem? Parent { get; set; }
    public int Position { get; set; }
    public RowType Type { get; set; }
    public double? Quantity { get; set; }
    public List<SheetItem> Children { get; set; } = new List<SheetItem>();
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


    [NotMapped]
    public double? UnitCost => Type == RowType.CostBearer ? CostBearerUnitCost : Children.Sum(x => x.TotalCost);
    [NotMapped]
    public double? TotalCost => UnitCost * (Quantity is null ? 1 : Quantity);
    [NotMapped]
    public double? UnitAskingPrice => Type == RowType.CostBearer ? CostBearerUnitAskingPrice : Children.Sum(x => x.TotalAskingPrice);
    [NotMapped]
    public double? TotalAskingPrice => UnitAskingPrice * (Quantity is null ? 1 : Quantity);

    public IEnumerable<SheetItem> AllInOrder
    {
        get
        {
            return new[] { this }.Concat(Children.SelectMany(x => x.AllInOrder).OrderBy(x => x.Position));
        }
    }
    public override string ToString()
    {
        return Description;
    }
}