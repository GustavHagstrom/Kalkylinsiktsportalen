namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

internal class StructureEstimationDto
{
    public StructureEstimationNodeDto? None { get; set; }

    public StructureEstimationNodeDto? NetSheet { get; set; }

    public StructureEstimationNodeDto? TenderSheet { get; set; }
    public StructureEstimationNodeDto? PrelimsSheet { get; set; }
    public StructureEstimationNodeDto? SummarySheet { get; set; }

    public StructureEstimationNodeDto? LockedStagesSheet { get; set; }

    public StructureEstimationNodeDto? ReconciliationSheet { get; set; }
}
