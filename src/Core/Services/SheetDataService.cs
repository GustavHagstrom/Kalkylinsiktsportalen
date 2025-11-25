using Core.Models.EstimationEnteties;
using Core.Enums.ViewTemplate;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Core.Services;
public class SheetDataService(IStringLocalizer<SheetDataService> Localizer) : ISheetDataService
{
    public string? GetColumnDataFromItem(SheetColumnType type, SheetItem item)
    {
        return type switch
        {
            SheetColumnType.Description => item.Description,
            SheetColumnType.Quantity => item.Quantity.ToString(),
            SheetColumnType.Unit => item.Unit,
            SheetColumnType.UnitCost => item.UnitCost is null ? string.Empty : ((double)item.UnitCost).ToString("N1"),
            SheetColumnType.TotalCost => item.TotalCost is null ? string.Empty : ((double)item.TotalCost).ToString("N1"),
            SheetColumnType.UnitAskingPrice => item.UnitAskingPrice is null ? string.Empty : ((double)item.UnitAskingPrice).ToString("N1"),
            SheetColumnType.TotalAskingPrice => item.TotalAskingPrice is null ? string.Empty : ((double)item.TotalAskingPrice).ToString("N1"),
            _ => throw new NotImplementedException()
        };
    }
    public string GetRowTypeName(SheetRowType type)
    {
        return type switch
        {
            SheetRowType.Header => Localizer["Tabellrubrik"],
            SheetRowType.Group => Localizer["Grupp"],
            SheetRowType.Part => Localizer["Del"],
            SheetRowType.Post => Localizer["Post"],
            SheetRowType.ChangedRow => Localizer["Ändrad rad"],
            _ => throw new NotImplementedException()
        };
    }
    public string GetColumnTypeName
        
        (SheetColumnType type)
    {
        return type switch
        {
            SheetColumnType.Description => Localizer["Benämning"],
            SheetColumnType.Quantity => Localizer["Mängd"],
            SheetColumnType.TotalAskingPrice => Localizer["Total kundpris"],
            SheetColumnType.TotalCost => Localizer["Total kostnad"],
            SheetColumnType.Unit => Localizer["Enhet"],
            SheetColumnType.UnitAskingPrice => Localizer["Kundpris / enhet"],
            SheetColumnType.UnitCost => Localizer["Kostnad / enhet"],
            _ => throw new NotImplementedException()
        };
    }
}
