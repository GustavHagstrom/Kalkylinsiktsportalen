using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BlazorServer.Modules.Reports.Persistence.MongoDb;

public class MongoView
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PageFormatName { get; set; } = string.Empty;
    public decimal PageFormatWidthMm { get; set; } = 0;
    public decimal PageFormatHeightMm { get; set; } = 0;
    public bool EnableChanges { get; set; } = false;
    public bool IsArchived { get; set; } = false;
    public List<MongoDataSection> DataSections { get; set; } = new();
    public List<MongoSheetSection> SheetSections { get; set; } = new();
}
