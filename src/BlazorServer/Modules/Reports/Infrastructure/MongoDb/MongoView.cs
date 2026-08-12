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
    public string PageFormatName { get; set; } = string.Empty;
    public decimal PageFormatWidthMm { get; set; } = 0;
    public decimal PageFormatHeightMm { get; set; } = 0;
    public bool EnableChanges { get; set; } = false;
    public bool IsArchived { get; set; } = false;
    public List<DataSection> DataSections { get; set; } = new();
    public List<SheetSection> SheetSections { get; set; } = new();
}
