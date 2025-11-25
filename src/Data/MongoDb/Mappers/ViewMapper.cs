using Models = Core.Models.EstimationView;
using Mongo = Data.MongoDb.Enteties.EstimationView;

namespace Data.MongoDb.Mappers;

public static class ViewMapper
{
    public static Models.View ToModel(Mongo.View mongoView)
    {
        var view = new Models.View
        {
            Id = mongoView.Id,
            TenantId = mongoView.TenantId,
            Name = mongoView.Name,
            AllowChanges = mongoView.AllowChanges,
            FontFamily = mongoView.FontFamily,
        };
        view.DataSections = DataSectionsToModel(mongoView.DataSections, view);
        view.SheetSections = SheetSectionsToModel(mongoView.SheetSections, view);
        view.PageBreakSections = PageBreakSectionsToModel(mongoView.PageBreakSections, view);
        view.Tags = TagsToModel(mongoView.Tags, view);
        return view;
    }
    public static Mongo.View ToMongo(Models.View modelView)
    {
        var view = new Mongo.View()
        {
            Id = modelView.Id,
            TenantId = modelView.TenantId,
            Name = modelView.Name,
            AllowChanges = modelView.AllowChanges,
            FontFamily = modelView.FontFamily,
            DataSections = DataSectionsToMongo(modelView.DataSections),
            SheetSections = SheetSectionsToMongo(modelView.SheetSections),
            PageBreakSections = PageBreakSectionsToMongo(modelView.PageBreakSections),
            Tags = TagsToMongo(modelView.Tags),
        };


        return view;
    }

    #region Tags
    private static List<Mongo.Tag> TagsToMongo(List<Models.Tag> modelTags)
    {
        var tags = modelTags
            .Select(mongoTag =>
            {
                var tag = new Mongo.Tag
                {
                    Name = mongoTag.Name,
                    Value = mongoTag.Value,
                    Type = mongoTag.Type,
                };
                return tag;
            })
            .ToList();
        return tags;
    }
    private static List<Models.Tag> TagsToModel(List<Mongo.Tag> mongoTags, Models.View view)
    {
        var tags = mongoTags
            .Select(mongoTag =>
            {
                var tag = new Models.Tag
                {
                    ViewId = view.Id,
                    View = view,
                    Name = mongoTag.Name,
                    Value = mongoTag.Value,
                    Type = mongoTag.Type,
                };
                return tag;
            })
            .ToList();
        return tags;
    }
    #endregion
    #region PageBreakSection
    private static List<Mongo.PageBreakSection> PageBreakSectionsToMongo(List<Models.PageBreakSection> modelPageBreakSections)
    {
        var sections = modelPageBreakSections
            .Select(modelPageBreakSection =>
            {
                var section = new Mongo.PageBreakSection
                {
                    Order = modelPageBreakSection.Order,
                    Name = modelPageBreakSection.Name,
                };
                return section;
            })
            .ToList();
        return sections;
    }
    private static List<Models.PageBreakSection> PageBreakSectionsToModel(List<Mongo.PageBreakSection> mongoPageBreakSections, Models.View view)
    {
        var sections = mongoPageBreakSections
            .Select(mongoPageBreakSection =>
            {
                var section = new Models.PageBreakSection
                {
                    ViewId = view.Id,
                    View = view,
                    Order = mongoPageBreakSection.Order,
                    Name = mongoPageBreakSection.Name,
                };
                return section;
            })
            .ToList();
        return sections;
    }
    #endregion
    #region SheetSection
    private static List<Mongo.SheetSection> SheetSectionsToMongo(List<Models.SheetSection> modelSheetSections)
    {
        var sections = modelSheetSections
            .Select(modelSheetSection =>
            {
                var section = new Mongo.SheetSection
                {
                    Order = modelSheetSection.Order,
                    Name = modelSheetSection.Name,
                    SheetType = modelSheetSection.SheetType,
                    Columns = SheetColumnsToMongo(modelSheetSection.Columns),
                    CellFormats = SheetCellFormatsToMongo(modelSheetSection.CellFormats)
                };
                return section;
            })
            .ToList();
        return sections;
    }
    private static List<Mongo.SheetColumn> SheetColumnsToMongo(List<Models.SheetColumn> modelSheetColumns)
    {
        var columns = modelSheetColumns
            .Select(modelSheetColumn =>
            {
                var column = new Mongo.SheetColumn
                {
                    Order = modelSheetColumn.Order,
                    ColumnType = modelSheetColumn.ColumnType,
                    IsVisible = modelSheetColumn.IsVisible,
                    Width = modelSheetColumn.Width,
                };
                return column;
            })
            .ToList();
        return columns;
    }
    private static List<Mongo.SheetCellFormat> SheetCellFormatsToMongo(List<Models.SheetCellFormat> modelSheetCellFormats)
    {
        var cellFormats = modelSheetCellFormats
            .Select(modelSheetCellFormat =>
            {
                var cellFormat = new Mongo.SheetCellFormat
                {
                    ColumnType = modelSheetCellFormat.ColumnType,
                    RowType = modelSheetCellFormat.RowType,
                    BorderStyle = modelSheetCellFormat.BorderStyle,
                    BackgroundColor = modelSheetCellFormat.BackgroundColor,
                    FontSize = modelSheetCellFormat.FontSize,
                    HorizontalAlign = modelSheetCellFormat.HorizontalAlign,
                    VerticalAlign = modelSheetCellFormat.VerticalAlign,
                    DecimalCount = modelSheetCellFormat.DecimalCount,
                    DoesIncludeTimeOfDay = modelSheetCellFormat.DoesIncludeTimeOfDay,
                    FormatType = modelSheetCellFormat.FormatType,
                    HasBorderBottom = modelSheetCellFormat.HasBorderBottom,
                    HasBorderLeft = modelSheetCellFormat.HasBorderLeft,
                    HasBorderRight = modelSheetCellFormat.HasBorderRight,
                    HasBorderTop = modelSheetCellFormat.HasBorderTop,
                    HasThoasandsSeparator = modelSheetCellFormat.HasThoasandsSeparator,
                    IsBold = modelSheetCellFormat.IsBold,
                    IsItalic = modelSheetCellFormat.IsItalic,
                    IsUnderline = modelSheetCellFormat.IsUnderline,
                    IsVisible = modelSheetCellFormat.IsVisible,
                    TextColor = modelSheetCellFormat.TextColor,
                };
                return cellFormat;
            })
            .ToList();
        return cellFormats;
    }

    private static List<Models.SheetSection> SheetSectionsToModel(List<Mongo.SheetSection> mongoSheetSections, Models.View view)
    {
        var sections = mongoSheetSections
            .Select(mongoSheetSection =>
            {
                var section = new Models.SheetSection
                {
                    ViewId = view.Id,
                    View = view,
                    Order = mongoSheetSection.Order,
                    Name = mongoSheetSection.Name,
                    SheetType = mongoSheetSection.SheetType,
                };
                section.Columns = SheetColumnsToModel(mongoSheetSection.Columns, section);
                section.CellFormats = SheetCellFormatsToModel(mongoSheetSection.CellFormats, section);
                return section;
            }).ToList();
        return sections;
    }
    private static List<Models.SheetColumn> SheetColumnsToModel(List<Mongo.SheetColumn> mongoSheetColumns, Models.SheetSection section)
    {
        var columns = mongoSheetColumns
            .Select(mongoSheetColumn =>
            {
                var column = new Models.SheetColumn
                {
                    SheetSectionId = section.Id,
                    SheetSection = section,
                    Order = mongoSheetColumn.Order,
                    ColumnType = mongoSheetColumn.ColumnType,
                    IsVisible = mongoSheetColumn.IsVisible,
                    Width = mongoSheetColumn.Width,
                };
                return column;
            })
            .ToList();
        return columns;
    }
    private static List<Models.SheetCellFormat> SheetCellFormatsToModel(List<Mongo.SheetCellFormat> mongoSheetCellFormats, Models.SheetSection section)
    {
        var cellFormats = mongoSheetCellFormats
            .Select(mongoSheetCellFormat =>
            {
                var cellFormat = new Models.SheetCellFormat
                {
                    SheetSectionId = section.Id,
                    SheetSection = section,
                    ColumnType = mongoSheetCellFormat.ColumnType,
                    RowType = mongoSheetCellFormat.RowType,
                    BorderStyle = mongoSheetCellFormat.BorderStyle,
                    BackgroundColor = mongoSheetCellFormat.BackgroundColor,
                    FontSize = mongoSheetCellFormat.FontSize,
                    HorizontalAlign = mongoSheetCellFormat.HorizontalAlign,
                    VerticalAlign = mongoSheetCellFormat.VerticalAlign,
                    DecimalCount = mongoSheetCellFormat.DecimalCount,
                    DoesIncludeTimeOfDay = mongoSheetCellFormat.DoesIncludeTimeOfDay,
                    FormatType = mongoSheetCellFormat.FormatType,
                    HasBorderBottom = mongoSheetCellFormat.HasBorderBottom,
                    HasBorderLeft = mongoSheetCellFormat.HasBorderLeft,
                    HasBorderRight = mongoSheetCellFormat.HasBorderRight,
                    HasBorderTop = mongoSheetCellFormat.HasBorderTop,
                    HasThoasandsSeparator = mongoSheetCellFormat.HasThoasandsSeparator,
                    IsBold = mongoSheetCellFormat.IsBold,
                    IsItalic = mongoSheetCellFormat.IsItalic,
                    IsUnderline = mongoSheetCellFormat.IsUnderline,
                    IsVisible = mongoSheetCellFormat.IsVisible,
                    TextColor = mongoSheetCellFormat.TextColor,
                };
                return cellFormat;
            })
            .ToList();
        return cellFormats;
    }
    #endregion
    #region DataSection
    private static List<Mongo.DataSection> DataSectionsToMongo(List<Models.DataSection> modelDataSection)
    {
        var sections = modelDataSection
            .Select(modelDataSection =>
            {
                var section = new Mongo.DataSection
                {
                    Order = modelDataSection.Order,
                    Name = modelDataSection.Name,
                    IsFooter = modelDataSection.IsFooter,
                    IsHeader = modelDataSection.IsHeader,
                    Columns = DataColumnToMongo(modelDataSection.Columns),
                    Rows = DataRowToMongo(modelDataSection.Rows),
                    Cells = DataCellToMongo(modelDataSection.Cells)
                };
                return section;
            })
            .ToList();
        return sections;
    }
    private static List<Mongo.DataColumn> DataColumnToMongo(List<Models.DataColumn> modelDataColumns)
    {
        var columns = modelDataColumns
            .Select(modelDataColumn =>
            {
                var column = new Mongo.DataColumn
                {
                    Order = modelDataColumn.Order,
                    Width = modelDataColumn.Width,
                };
                return column;
            })
            .ToList();
        return columns;
    }
    private static List<Mongo.DataRow> DataRowToMongo(List<Models.DataRow> modelDataRow)
    {
        var rows = modelDataRow
            .Select(modelDataRow =>
            {
                var row = new Mongo.DataRow
                {
                    Order = modelDataRow.Order,
                    Height = modelDataRow.Height,
                };
                return row;
            })
            .ToList();
        return rows;
    }
    private static List<Mongo.DataCell> DataCellToMongo(List<Models.DataCell> modelDataCells)
    {
        var cells = modelDataCells
            .Select(modelDataCell =>
            {
                var cell = new Mongo.DataCell
                {
                    Row = modelDataCell.Row,
                    Column = modelDataCell.Column,
                    Formula = modelDataCell.Formula,
                    CellFormat = new(),
                };
                if (modelDataCell.CellFormat is not null)
                {
                    cell.CellFormat = new Mongo.DataCellFormat
                    {
                        HorizontalAlign = modelDataCell.CellFormat.HorizontalAlign,
                        VerticalAlign = modelDataCell.CellFormat.VerticalAlign,
                        FontSize = modelDataCell.CellFormat.FontSize,
                        IsBold = modelDataCell.CellFormat.IsBold,
                        IsItalic = modelDataCell.CellFormat.IsItalic,
                        IsUnderline = modelDataCell.CellFormat.IsUnderline,
                        FormatType = modelDataCell.CellFormat.FormatType,
                        HasThoasandsSeparator = modelDataCell.CellFormat.HasThoasandsSeparator,
                        DecimalCount = modelDataCell.CellFormat.DecimalCount,
                        DoesIncludeTimeOfDay = modelDataCell.CellFormat.DoesIncludeTimeOfDay,
                        BorderStyle = modelDataCell.CellFormat.BorderStyle,
                        BackgroundColor = modelDataCell.CellFormat.BackgroundColor,
                        FontFamily = modelDataCell.CellFormat.FontFamily,
                        HasBorderBottom = modelDataCell.CellFormat.HasBorderBottom,
                        HasBorderLeft = modelDataCell.CellFormat.HasBorderLeft,
                        HasBorderRight = modelDataCell.CellFormat.HasBorderRight,
                        HasBorderTop = modelDataCell.CellFormat.HasBorderTop,
                        TextColor = modelDataCell.CellFormat.TextColor,
                    };
                }
                return cell;
            })
            .ToList();
        return cells;
    }
    private static List<Models.DataSection> DataSectionsToModel(List<Mongo.DataSection> mongoDataSections, Models.View view)
    {
        var sections = mongoDataSections
            .Select(mongoDataSection =>
            {
                var section = new Models.DataSection
                {
                    ViewId = view.Id,
                    View = view,
                    Order = mongoDataSection.Order,
                    Name = mongoDataSection.Name,
                    IsFooter = mongoDataSection.IsFooter,
                    IsHeader = mongoDataSection.IsHeader,
                };
                section.Columns = DataColumnToModel(mongoDataSection.Columns, section);
                section.Rows = DataRowToModel(mongoDataSection.Rows, section);
                section.Cells = DataCellToModel(mongoDataSection.Cells, section);
                return section;
            })
            .ToList();
        return sections;
    }
    private static List<Models.DataColumn> DataColumnToModel(List<Mongo.DataColumn> mongoDataColumns, Models.DataSection section)
    {
        var columns = mongoDataColumns
            .Select(mongoDataColumn =>
            {
                var column = new Models.DataColumn
                {
                    DataSectionId = section.Id,
                    DataSection = section,
                    Order = mongoDataColumn.Order,
                    Width = mongoDataColumn.Width,
                };
                return column;
            })
            .ToList();
        return columns;
    }
    private static List<Models.DataRow> DataRowToModel(List<Mongo.DataRow> mongoDataRow, Models.DataSection section)
    {
        var rows = mongoDataRow
            .Select(mongoDataRow =>
            {
                var row = new Models.DataRow
                {
                    DataSectionId = section.Id,
                    DataSection = section,
                    Order = mongoDataRow.Order,
                    Height = mongoDataRow.Height,
                };
                return row;
            })
            .ToList();
        return rows;
    }
    private static List<Models.DataCell> DataCellToModel(List<Mongo.DataCell> mongoDataCells, Models.DataSection section)
    {
        var cells = mongoDataCells
            .Select(mongoDataCell =>
            {
                var cell = new Models.DataCell
                {
                    DataSectionId = section.Id,
                    DataSection = section,
                    Row = mongoDataCell.Row,
                    Column = mongoDataCell.Column,
                    Formula = mongoDataCell.Formula,
                };
                cell.CellFormat = new Models.DataCellFormat
                {
                    DataCellId = cell.Id,
                    DataCell = cell,
                    HorizontalAlign = mongoDataCell.CellFormat.HorizontalAlign,
                    VerticalAlign = mongoDataCell.CellFormat.VerticalAlign,
                    FontSize = mongoDataCell.CellFormat.FontSize,
                    IsBold = mongoDataCell.CellFormat.IsBold,
                    IsItalic = mongoDataCell.CellFormat.IsItalic,
                    IsUnderline = mongoDataCell.CellFormat.IsUnderline,
                    FormatType = mongoDataCell.CellFormat.FormatType,
                    HasThoasandsSeparator = mongoDataCell.CellFormat.HasThoasandsSeparator,
                    DecimalCount = mongoDataCell.CellFormat.DecimalCount,
                    DoesIncludeTimeOfDay = mongoDataCell.CellFormat.DoesIncludeTimeOfDay,
                    BorderStyle = mongoDataCell.CellFormat.BorderStyle,
                    BackgroundColor = mongoDataCell.CellFormat.BackgroundColor,
                    FontFamily = mongoDataCell.CellFormat.FontFamily,
                    HasBorderBottom = mongoDataCell.CellFormat.HasBorderBottom,
                    HasBorderLeft = mongoDataCell.CellFormat.HasBorderLeft,
                    HasBorderRight = mongoDataCell.CellFormat.HasBorderRight,
                    HasBorderTop = mongoDataCell.CellFormat.HasBorderTop,
                    TextColor = mongoDataCell.CellFormat.TextColor,
                };
                return cell;
            })
            .ToList();
        return cells;
    }
    #endregion
}
