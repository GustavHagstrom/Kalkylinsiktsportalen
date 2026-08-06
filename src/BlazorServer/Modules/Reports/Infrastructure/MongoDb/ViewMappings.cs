using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Identity;

namespace BlazorServer.Modules.Reports.Infrastructure.MongoDb;

public static class ViewMappings
{
    public static ViewDto ToDto(this MongoView mongoView)
    {
        return new ViewDto
        {
            Id = new ViewId(mongoView.Id),
            Name = mongoView.Name,
            OrganizationId = new OrganizationId(mongoView.OrganizationId),
            SheetSections = mongoView.SheetSections,
            DataSections = mongoView.DataSections,
            Description = mongoView.Description,
        };
    }
    public static MongoView ToMongo(this ViewDto viewDto)
    {
        return new MongoView
        {
            Id = viewDto.Id.IsEmpty ? ViewId.New().Value : viewDto.Id.Value,
            Name = viewDto.Name,
            OrganizationId = viewDto.OrganizationId.Value,
            SheetSections = viewDto.SheetSections,
            DataSections = viewDto.DataSections,
            Description = viewDto.Description,
        };
    }
}
