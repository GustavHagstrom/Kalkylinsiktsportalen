namespace BlazorServer.Features.BidconBrowser;

public class EstimationInfo
{
    public EstimationInfo(string Name, string BidconId, bool IsSelected)
    {
        this.Name = Name;
        this.BidconId = BidconId;
        this.IsSelected = IsSelected;
    }

    public string Name { get; }
    public string BidconId { get; }
    public bool IsSelected { get; set; }
}