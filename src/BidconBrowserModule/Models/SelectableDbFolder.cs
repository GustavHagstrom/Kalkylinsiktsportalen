using Core.Models.FromShared;

namespace BidconBrowserModule.Models;
public class SelectableDbFolder
{
    private bool _isSelected;


    public SelectableDbFolder(Folder dbFolder, bool isSelected = false)
    {
        Name = dbFolder.Name;
        _isSelected = isSelected;
        SetSubFolders(dbFolder);
        SetEstimations(dbFolder);
    }
    public string Name { get; set; }
    public List<SelectableDbFolder> SubFolders { get; set; } = new();
    public List<SelectableDbEstimation> Estimations { get; set; } = new();
    public bool IsSelected
    {
        get => _isSelected; set
        {
            _isSelected = value;
            SetAllSubItems(value);
            NotifySelectionChanged();
        }
    }

    private void SetEstimations(Folder dbFolder)
    {
        foreach (var estimation in dbFolder.DbEstimations)
        {
            var newItem = new SelectableDbEstimation(estimation);
            Estimations.Add(newItem);
            newItem.SelectionChanged += OnSubItemSelectionChanged;
        }
    }
    private void SetSubFolders(Folder dbFolder)
    {
        foreach (var folder in dbFolder.SubFolders)
        {
            var newItem = new SelectableDbFolder(folder);
            SubFolders.Add(newItem);
            newItem.SelectionChanged += OnSubItemSelectionChanged;
        }
    }
    private void SetAllSubItems(bool value)
    {
        foreach (var folder in SubFolders)
        {
            folder.SetIsSelectedWithoutNotification(value);
        }
        foreach (var estimation in Estimations)
        {
            estimation.SetIsSelectedWithoutNotification(value);
        }
    }
    private void OnSubItemSelectionChanged()
    {
        var anySubItemIsSelected = SubFolders.Any(x => x.IsSelected) || Estimations.Any(x => x.IsSelected);
        _isSelected = anySubItemIsSelected;
        NotifySelectionChanged();
    }
    public IEnumerable<SelectableDbEstimation> GetAllEstimations()
    {
        foreach (var folder in SubFolders)
        {
            foreach (var estimation in folder.GetAllEstimations())
            {
                yield return estimation;
            }
        }
        foreach (var estimation in Estimations)
        {
            yield return estimation;
        }
    }
    public event Action? SelectionChanged;
    private void NotifySelectionChanged() => SelectionChanged?.Invoke();
    public void SetIsSelectedWithoutNotification(bool value)
    {
        _isSelected = value;
    }
}
