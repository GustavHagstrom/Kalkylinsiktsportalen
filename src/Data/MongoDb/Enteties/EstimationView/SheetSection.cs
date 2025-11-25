using Core.Enums.BidconAccess;

namespace Data.MongoDb.Enteties.EstimationView;

public class SheetSection
{
    public string Name { get; set; } = "SheetSection";
    public int Order { get; set; }
    public SheetType SheetType { get; set; }
    public List<SheetColumn> Columns { get; set; } = new();
    public List<SheetCellFormat> CellFormats { get; set; } = new();

}
