using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;

namespace BlazorServer.Modules.Reports.Persistence.MongoDb;

public sealed class MongoDataSection
{
    public int Order { get; set; }
    public SectionId SectionId { get; set; } = SectionId.New();
    public PageBreakBehavior PageBreakBehavior { get; set; } = PageBreakBehavior.None;
    public List<RowDefinition> Rows { get; set; } = [];
    public List<ColumnDefinition> Columns { get; set; } = [];
    public Dictionary<DataCellKey, DataCell> DataCells { get; set; } = [];
}
