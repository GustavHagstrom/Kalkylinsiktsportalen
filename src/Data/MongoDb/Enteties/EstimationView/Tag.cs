using Core.Enums.ViewTemplate;

namespace Data.MongoDb.Enteties.EstimationView;

public class Tag 
{
    public string Name { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public TagType Type { get; set; }
}
