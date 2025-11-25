namespace Data.MongoDb.Enteties.FromShared;
public class Folder
{
    public int FolderNum { get; set; }
    public int ParentNum { get; set; }
    public required string Name { get; set; }
    public required ICollection<Folder> SubFolders { get; set; }
    public required ICollection<EstimationInfo> DbEstimations { get; set; }
    public IEnumerable<EstimationInfo> GetAllEstimations()
    {
        return GetAllEstimations(this);
    }
    private IEnumerable<EstimationInfo> GetAllEstimations(Folder folder)
    {
        foreach (var subFolder in folder.SubFolders)
        {
            foreach (var estimation in GetAllEstimations(subFolder))
            {
                yield return estimation;
            }
        }
        foreach (var estimation in folder.DbEstimations)
        {
            yield return estimation;
        }
    }
    public override string ToString() => Name;
}
