using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Modules.Reports.Infrastructure.MongoDb;

public class MongoView
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataSectionDto> DataSections { get; set; } = new();
    public List<SheetSectionDto> SheetSections { get; set; } = new();
}
