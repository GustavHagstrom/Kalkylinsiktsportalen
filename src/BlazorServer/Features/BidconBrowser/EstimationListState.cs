namespace BlazorServer.Features.BidconBrowser;

public class EstimationListState
{
    public bool HasLoaded { get; set; } = false;
    public List<EstimationInfo> Estimations { get; set; } = [];
    public List<EstimationInfo> SelectedEstimations { get; set; } = [];
}
