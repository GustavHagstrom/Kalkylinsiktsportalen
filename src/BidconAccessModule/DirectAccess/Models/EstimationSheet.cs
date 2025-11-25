namespace BidconAccessModule.DirectAccess.Models;
internal class EstimationSheet
{
    public Guid EstimationId { get; set; }
    public required string LayerId { get; set; }
    public int Row { get; set; }
    public int ParentRow { get; set; }
    public int Position { get; set; }
    public required string Description { get; set; }
    public required string Remark { get; set; }
    public double? Quantity { get; set; }
    public required string Unit { get; set; }
    public int RowType { get; set; }
    public int SheetType { get; set; }
    public int? LayerType { get; set; }
    public string? RevisionCode { get; set; }
    public int? PMATANum { get; set; }
    public int? AddedInPhase { get; set; }
    public double? UnitPriceManual { get; set; }
}
