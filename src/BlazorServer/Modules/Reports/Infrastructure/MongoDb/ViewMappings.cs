using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Reports.Infrastructure.MongoDb;

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
            PageFormat.FromPersistedName(mongoView.PageFormatName),
            mongoView.EnableChanges,
            mongoView.IsArchived,
            mongoView.DataSections,
            mongoView.SheetSections);
    }
    public static MongoView ToMongo(this View view)
    {
        return new MongoView
        {
            Id = view.Id.IsEmpty ? ViewId.New().Value : view.Id.Value,
            Name = view.Name,
            OrganizationId = view.OrganizationId.Value,
            SheetSections = view.LegacySheetSections.ToList(),
            DataSections = view.LegacyDataSections.ToList(),
            Description = view.Description,
            PageFormatName = view.PageFormat.Name,
            PageFormatWidthMm = view.PageFormat.WidthMm,
            PageFormatHeightMm = view.PageFormat.HeightMm,
            EnableChanges = false,
            IsArchived = view.IsArchived,
        };
    }
}
