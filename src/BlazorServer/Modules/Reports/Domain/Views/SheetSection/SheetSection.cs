using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Domain.Views.SheetSection;

public class SheetSection
{
    public SheetSection()
    {
        foreach (var rowType in Enum.GetValues<RowType>())
        {
            foreach (var columnType in Enum.GetValues<ColumnType>())
            {
                var key = $"{rowType}-{columnType}";
                SheetCells[key] = new SheetCell
                {
                    RowType = rowType,
                    ColumnType = columnType
                };
            }
        }
    }
    public string SectionId { get; set; } = SectionIds.New();
    public int SectionIndex { get; set; } = 0;
    public string Content { get; set; } = string.Empty;
    public bool EnableChanges { get; set; } = false;
    /// <summary>
    /// Key: $"{rowType}-{columnType}"
    /// </summary>
    public Dictionary<string, SheetCell> SheetCells { get; set; } = new();

    public void SetEnableChanges(bool enableChanges)
    {
        EnableChanges = enableChanges;
    }

    public void Normalize()
    {
        if (string.IsNullOrWhiteSpace(SectionId))
            SectionId = SectionIds.New();

        SheetCells ??= new Dictionary<string, SheetCell>();
    }
}
