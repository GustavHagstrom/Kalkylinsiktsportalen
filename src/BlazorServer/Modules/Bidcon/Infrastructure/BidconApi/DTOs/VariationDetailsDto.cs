namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

internal class VariationDetailsDto : VariationDto
{
    public List<ResourceDto> Resources { get; set; } = [];
}
