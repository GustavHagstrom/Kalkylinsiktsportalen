namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

internal class EstimationDetailsDto : EstimationDto
{
    public List<ResourceDto> BidPhaseResources { get; set; } = [];

    public List<ResourceDto> ProductionPhaseResources { get; set; } = [];

    public List<VariationDetailsDto> Variations { get; set; } = [];

    public Structure? BidPhaseStructures { get; set; }

    public Structure? VariationProductionPhaseStructures { get; set; }

    public List<DynamicPropertiesDto> DynamicProperties { get; set; } = [];

    public double TenderRateExVAT { get; set; }
    
    public double TenderRateIncVAT { get; set; }
    
    public bool IsCompleted { get; set; }
}
