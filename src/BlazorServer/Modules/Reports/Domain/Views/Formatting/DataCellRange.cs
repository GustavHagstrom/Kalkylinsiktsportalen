namespace BlazorServer.Modules.Reports.Domain.Views;

public readonly record struct DataCellRange(int StartRowIndex, int StartColumnIndex, int EndRowIndex, int EndColumnIndex)
{
    public static DataCellRange FromBounds(int startRowIndex, int startColumnIndex, int endRowIndex, int endColumnIndex)
        => new(startRowIndex, startColumnIndex, endRowIndex, endColumnIndex);
}
