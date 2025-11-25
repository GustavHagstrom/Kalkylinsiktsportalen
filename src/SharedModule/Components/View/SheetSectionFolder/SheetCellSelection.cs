using Core.Models.EstimationEnteties;
using Core.Enums.ViewTemplate;

namespace SharedModule.Components.View.SheetSectionFolder;

public record SheetCellSelection(SheetItem? Item, SheetColumnType ColumnType);
