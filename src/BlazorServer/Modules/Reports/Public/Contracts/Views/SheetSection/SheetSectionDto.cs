namespace BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection;

public class SheetSectionDto
{
    public SheetSectionDto()
    {
        foreach (var rowType in Enum.GetValues<RowType>())
        {
            foreach (var columnType in Enum.GetValues<ColumnType>())
            {
                var key = $"{rowType}-{columnType}";
                SheetCells[key] = new SheetCellDto
                {
                    RowType = rowType,
                    ColumnType = columnType
                };
            }
        }
    }
    public int SectionIndex { get; set; } = 0;
    public string Content { get; set; } = string.Empty;
    /// <summary>
    /// Key: $"{rowType}-{columnType}"
    /// </summary>
    public Dictionary<string, SheetCellDto> SheetCells { get; set; } = new();
}
