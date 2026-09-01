using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;

namespace BlazorServer.Modules.Reports.Persistence.MongoDb;

public sealed class MongoSheetSection
{
    public int Order { get; set; }
    public SectionId SectionId { get; set; } = SectionId.New();
    public bool ShowHeaders { get; set; } = true;
    public SheetKind SheetKind { get; set; } = SheetKind.TenderSheet;
    public SheetColumnConfiguration[] Columns { get; set; } = [];
    public bool EnableChanges { get; set; } = false;
    public Dictionary<SheetCellKey, SheetCell> SheetCells { get; set; } = [];
    public PageBreakBehavior PageBreakBehavior { get; set; } = PageBreakBehavior.None;
}
