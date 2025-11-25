namespace Data.MongoDb.Enteties.FromShared;
public class EstimationInfo
{
    public int FolderNum { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
    public override string ToString() => Description is null ? Name : $"{Name} - {Description}";
}
