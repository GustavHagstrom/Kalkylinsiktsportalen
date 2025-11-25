using Data.MongoDb.Enteties.Abstractions;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.MongoDb.Enteties.EstimationView;
public class View
{
    [BsonId]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<DataSection> DataSections { get; set; } = [];
    public List<SheetSection> SheetSections { get; set; } = [];
    public List<PageBreakSection> PageBreakSections { get; set; } = [];
    public string FontFamily { get; set; } = "Calibri";
    public bool AllowChanges { get; set; } = true;
    public List<Tag> Tags { get; set; } = [];
}
