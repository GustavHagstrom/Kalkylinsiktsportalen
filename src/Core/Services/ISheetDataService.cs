using Core.Models.EstimationEnteties;
using Core.Enums.ViewTemplate;

namespace Core.Services;
public interface ISheetDataService
{
    string? GetColumnDataFromItem(SheetColumnType type, SheetItem item);
    string GetColumnTypeName(SheetColumnType type);
    string GetRowTypeName(SheetRowType type);
}