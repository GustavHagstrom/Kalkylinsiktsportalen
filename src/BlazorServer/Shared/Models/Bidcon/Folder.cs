namespace BlazorServer.Shared.Models.Bidcon;

public class Folder
{
    public List<Folder> SubFolders { get; set; } = [];
    public List<Estimation> Estimations { get; set; } = [];
    public IEnumerable<Estimation> GetAllEstimations()
    {
        foreach (var estimation in Estimations)
        {
            yield return estimation;
        }
        foreach (var subFolder in SubFolders)
        {
            foreach (var estimation in subFolder.GetAllEstimations())
            {
                yield return estimation;
            }
        }
    }
}
