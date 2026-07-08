namespace BlazorServer.Modules.Bidcon.Components;

public class EstimationInfo
{
    public EstimationInfo(string name, string bidconId, bool isSelected)
    {
        Name = name;
        BidconId = bidconId;
        IsSelected = isSelected;
    }

    public string Name { get; }
    public string BidconId { get; }
    public bool IsSelected { get; set; }
}
