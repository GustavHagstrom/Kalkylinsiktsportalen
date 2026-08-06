namespace BlazorServer.Modules.Reports.Domain.Views.DataSection;

public class DataSectionDto
{
    public int SectionIndex { get; set; } = 0;
    public int RowsCount { get; set; } = 0;
    public int ColumnsCount { get; set; } = 0;
    /// <summary>
    /// Key: "R{rowIndex}C{columnIndex}" 0 based arrays
    /// </summary>
    public Dictionary<string, DataCellDto> DataCells { get; set; } = new();
    public List<RowDefinition> RowDefinitions { get; set; } = new();
    public List<DataColumnDefinition> ColumnDefinitions { get; set; } = new();

}
