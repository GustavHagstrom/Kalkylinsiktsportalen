namespace BlazorServer.Shared.DTOs.Bidcon;

public class VariationDetailsDto : VariationDto
{
    public List<ResourceDto> Resources { get; set; } = [];
}
