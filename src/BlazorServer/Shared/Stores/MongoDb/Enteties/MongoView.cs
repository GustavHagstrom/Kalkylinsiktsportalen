using BlazorServer.Shared.DTOs.Views;
using BlazorServer.Shared.DTOs.Views.DataSection;
using BlazorServer.Shared.DTOs.Views.SheetSection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Shared.Stores.MongoDb.Enteties;

public class MongoView
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataSectionDto> DataSections { get; set; } = new();
    public List<SheetSectionDto> SheetSections { get; set; } = new();
    public List<PageBreakSection> PageBreakSections { get; set; } = new();
}
