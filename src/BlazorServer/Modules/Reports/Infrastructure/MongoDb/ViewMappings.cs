using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSections;
using BlazorServer.Modules.Reports.Domain.Views.SheetSections;
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
            mongoView.IsArchived,
            mongoView.DataSections.Cast<IViewSection>()
                .Concat(mongoView.SheetSections)
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
            SheetSections = view.Sections.OfType<SheetSection>().ToList(),
            DataSections = view.Sections.OfType<DataSection>().ToList(),
            Description = view.Description,
            PageFormatName = view.PageFormat.Name,
            PageFormatWidthMm = view.PageFormat.WidthMm,
            PageFormatHeightMm = view.PageFormat.HeightMm,
            EnableChanges = false,
            IsArchived = view.IsArchived,
        };
    }
}
