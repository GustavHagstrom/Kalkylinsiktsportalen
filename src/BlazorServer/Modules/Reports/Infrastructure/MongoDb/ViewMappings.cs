using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Reports.Infrastructure.MongoDb;

public static class ViewMappings
{
    public static View ToDomain(this MongoView mongoView)
    {
        return new View
        {
            Id = new ViewId(mongoView.Id),
            Name = mongoView.Name,
            OrganizationId = new OrganizationId(mongoView.OrganizationId),
            SheetSections = mongoView.SheetSections,
            DataSections = mongoView.DataSections,
            Description = mongoView.Description,
        };
    }
    public static MongoView ToMongo(this View view)
    {
        return new MongoView
        {
            Id = view.Id.IsEmpty ? ViewId.New().Value : view.Id.Value,
            Name = view.Name,
            OrganizationId = view.OrganizationId.Value,
            SheetSections = view.SheetSections,
            DataSections = view.DataSections,
            Description = view.Description,
        };
    }
}
