using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Infrastructure.Persistence.Entities;
using MongoDB.Bson;

namespace BlazorServer.Modules.Reports.Infrastructure.Persistence.Mappers;

public static class MongoViewMapper
{
    public static ViewDto ToDto(this MongoView mongoView)
    {
        return new ViewDto
        {
            Id = mongoView.Id.ToString(),
            Name = mongoView.Name,
            OrganizationId = mongoView.OrganizationId,
            SheetSections = mongoView.SheetSections,
            DataSections = mongoView.DataSections,
            PageBreakSections = mongoView.PageBreakSections,
            Description = mongoView.Description,
        };
    }
    public static MongoView ToMongo(this ViewDto viewDto)
    {
        return new MongoView
        {
            Id = string.IsNullOrWhiteSpace(viewDto.Id) ? Guid.NewGuid().ToString() : viewDto.Id,
            Name = viewDto.Name,
            OrganizationId = viewDto.OrganizationId,
            SheetSections = viewDto.SheetSections,
            DataSections = viewDto.DataSections,
            PageBreakSections = viewDto.PageBreakSections,
            Description = viewDto.Description,
        };
    }
}
