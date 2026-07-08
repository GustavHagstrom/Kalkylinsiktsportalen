namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

public class VariationDetailsDto : VariationDto
{
    public List<ResourceDto> Resources { get; set; } = [];
}
