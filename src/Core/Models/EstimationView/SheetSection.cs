using Core.Enums.BidconAccess;
using Core.Enums.ViewTemplate;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;

public class SheetSection : IViewEntity, ISection
{
    public SheetSection()
    {
        Id = Guid.NewGuid().ToString();
        CellFormats = CreateCellFormats(Id).ToList();
    }
    [StringLength(50)]
    public string Id { get; set; }
    [StringLength(50)]
    public string Name { get; set; } = "SheetSection";
    public int Order { get; set; }
    public SheetType SheetType { get; set; }

    public List<SheetColumn> Columns { get; set; } = new();
    public List<SheetCellFormat> CellFormats { get; set; }

    [StringLength(450)]
    public string ViewId { get; set; } = string.Empty;
    public View? View { get; set; }


    private IEnumerable<SheetCellFormat> CreateCellFormats(string sectionId)
    {
        foreach (var columnType in Enum.GetValues<SheetColumnType>())
        {
            foreach (var rowType in Enum.GetValues<SheetRowType>())
            {
                yield return new SheetCellFormat
                {
                    SheetSectionId = sectionId,
                    ColumnType = columnType,
                    RowType = rowType
                };
            }
        }
    }
}
