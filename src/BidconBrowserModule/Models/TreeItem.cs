using Core.Models.FromShared;

namespace BidconBrowserModule.Models;
public class TreeItem
{
    private bool _isSelected;


    public TreeItem(Folder folder, bool isSelected = false)
    {
        Type = DbTreeItemType.Folder;
        _isSelected = isSelected;
        Name = folder.Name;
        AddEstimations(folder);
        AddSubFolders(folder);
    }
    public TreeItem(EstimationInfo estimation, bool isSelected = false)
    {
        Type = DbTreeItemType.Estimation;
        _isSelected = isSelected;
        Id = estimation.Id;
        Name = estimation.ToString();
        DbEstimation = estimation;
    }
    public EstimationInfo? DbEstimation { get; set; }
    public DbTreeItemType Type { get; private set; }
    public bool IsSelected
    {
        get => _isSelected; set
        {
            _isSelected = value;
            SetAllSubItemsWithoutNotification(value);
            NotifySelectionChanged();
        }
    }
    public string Id { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public HashSet<TreeItem> Items { get; private set; } = new();
    public Action? SelectionChanged { get; set; }
    public IEnumerable<TreeItem> GetAllEstimations()
    {
        foreach (var item in Items)
        {
            if (item.Type == DbTreeItemType.Estimation)
            {
                yield return item;
            }
            foreach (var subEstimation in item.GetAllEstimations())
            {
                yield return subEstimation;
            }
        }
    }
    public IEnumerable<TreeItem> SelectedEstimations()
    {
        return GetAllEstimations().Where(x => x.IsSelected == true);
    }
    public IEnumerable<TreeItem> SearchEstimations(string filterString)
    {
        var parameters = filterString.Split(" ");
        return GetAllEstimations().Where(x => AllParametersExists(x, parameters));
    }
    private static bool AllParametersExists(TreeItem estimationItem, IEnumerable<string> parameters)
    {
        foreach (var parameter in parameters)
        {
            if (!estimationItem.Name.ToLower().Contains(parameter.ToLower()))
            {
                return false;
            }
        }
        return true;
    }
    private void AddEstimations(Folder folder)
    {
        foreach (var estimation in folder.DbEstimations)
        {
            var newItem = new TreeItem(estimation);
            Items.Add(newItem);
            newItem.SelectionChanged = OnSubItemSelectionChanged;
        }
    }
    private void AddSubFolders(Folder folder)
    {
        foreach (var subFolder in folder.SubFolders)
        {
            var newItem = new TreeItem(subFolder);
            Items.Add(newItem);
            newItem.SelectionChanged = OnSubItemSelectionChanged;
        }
    }
    private void OnSubItemSelectionChanged()
    {
        _isSelected = Items.Any(x => x.IsSelected);
        NotifySelectionChanged();
    }
    private void SetAllSubItemsWithoutNotification(bool value)
    {
        foreach (var item in Items)
        {
            item.SetIsSelectedWithoutNotification(value);
        }
    }
    private void SetIsSelectedWithoutNotification(bool value)
    {
        _isSelected = value;
        SetAllSubItemsWithoutNotification(value);
    }
    private void NotifySelectionChanged()
    {
        SelectionChanged?.Invoke();
    }
}
