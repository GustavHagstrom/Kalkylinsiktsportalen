using Core.Models.FromShared;

namespace BidconBrowserModule.Models;
public class SelectableDbEstimation : EstimationInfo
{
    private bool _isSelected;


    public SelectableDbEstimation(EstimationInfo dbEstimation, bool isSelected = false)
    {
        Id = dbEstimation.Id;
        Name = dbEstimation.Name;
        Description = dbEstimation.Description;
        _isSelected = isSelected;
    }
    public bool IsSelected
    {
        get => _isSelected; set
        {
            _isSelected = value;
            NotifySelectionChanged();
        }
    }
    public void SetIsSelectedWithoutNotification(bool value)
    {
        _isSelected = value;
    }
    public event Action? SelectionChanged;
    private void NotifySelectionChanged()
    {
        SelectionChanged?.Invoke();
    }
}
