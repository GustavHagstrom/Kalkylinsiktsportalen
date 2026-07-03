namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot.ExternalDTOs;

public class Structure
{
    public StructureCodeNode? Accounts { get; set; }
    public StructureCodeNode? ResourceTypes { get; set; }
    public StructureCodeNode? DSCodes { get; set; }
    public StructureCodeNode? Locations { get; set; }
    public StructureEstimationDto? Estimation { get; set; }
    public StructureCodeNode? TradeCodes { get; set; }
}
