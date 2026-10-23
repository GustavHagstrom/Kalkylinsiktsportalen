using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Reports.Persistence.MongoDb;

public static class ViewMappings
{
    public static View? ToDomain(this MongoView? mongoView)
    {
        if (mongoView is null)
            return null;

        return View.Reconstitute(
            new ViewId(mongoView.Id),
            new OrganizationId(mongoView.OrganizationId),
            mongoView.Name,
            mongoView.Description,
            mongoView.IsArchived,
            mongoView.DataSections
                .Select(section => new OrderedViewSection(section.Order, Section: ToDomain(section)))
                .Concat(mongoView.SheetSections.Select(section => new OrderedViewSection(section.Order, ToDomain(section))))
                .OrderBy(section => section.Order)
                .Select(section => section.Section)
                .ToArray(),
            PageFormat.FromPersistedName(mongoView.PageFormatName));
    }
    public static MongoView ToMongo(this View view)
    {
        return new MongoView
        {
            Id = view.Id.IsEmpty ? ViewId.New().Value : view.Id.Value,
            Name = view.Name,
            OrganizationId = view.OrganizationId.Value,
            SheetSections = view.Sections
                .Select((section, index) => new { section, index })
                .Where(item => item.section is SheetSection)
                .Select(item => ToMongo((SheetSection)item.section, item.index))
                .ToList(),
            DataSections = view.Sections
                .Select((section, index) => new { section, index })
                .Where(item => item.section is DataSection)
                .Select(item => ToMongo((DataSection)item.section, item.index))
                .ToList(),
            Description = view.Description,
            PageFormatName = view.PageFormat.Name,
            PageFormatWidthMm = view.PageFormat.WidthMm,
            PageFormatHeightMm = view.PageFormat.HeightMm,
            EnableChanges = false,
            IsArchived = view.IsArchived,
        };
    }

    private static DataSection ToDomain(MongoDataSection section)
        => DataSection.Reconstitute(
            section.SectionId,
            section.Rows,
            section.Columns,
            section.DataCells,
            section.PageBreakBehavior);

    private static SheetSection ToDomain(MongoSheetSection section)
        => SheetSection.Reconstitute(
            section.SectionId,
            section.Columns,
            section.SheetKind,
            section.EnableChanges,
            section.ShowHeaders,
            section.SheetCellConfigurations,
            section.PageBreakBehavior);

    private static MongoDataSection ToMongo(DataSection section, int order)
        => new()
        {
            Order = order,
            SectionId = section.SectionId,
            PageBreakBehavior = section.PageBreakBehavior,
            Rows = section.Rows.ToList(),
            Columns = section.Columns.ToList(),
            DataCells = section.Cells.ToDictionary(
                cell => new DataCellKey(cell.RowIndex, cell.ColumnIndex),
                cell => cell)
        };

    private static MongoSheetSection ToMongo(SheetSection section, int order)
        => new()
        {
            Order = order,
            SectionId = section.SectionId,
            SheetKind = section.SheetKind,
            ShowHeaders = section.ShowHeaders,
            Columns = section.Columns.ToArray(),
            EnableChanges = section.EnableChanges,
            SheetCellConfigurations = section.SheetCellConfigurations.ToDictionary(
                cell => cell.Key,
                cell => cell.Value),
            PageBreakBehavior = section.PageBreakBehavior
        };

    private sealed record OrderedViewSection(int Order, IViewSection Section);
}
