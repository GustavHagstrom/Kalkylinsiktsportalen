namespace BlazorServer.Modules.Bidcon.Components;

public class EstimationListState
{
    public bool HasLoaded { get; set; }
    public List<EstimationInfo> Estimations { get; set; } = [];
    public List<EstimationInfo> SelectedEstimations { get; set; } = [];
}
