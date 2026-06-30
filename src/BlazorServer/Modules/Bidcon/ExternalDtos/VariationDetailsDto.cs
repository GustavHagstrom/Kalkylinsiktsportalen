namespace BlazorServer.Modules.Bidcon.ExternalDtos;

public class VariationDetailsDto : VariationDto
{
    public List<ResourceDto> Resources { get; set; } = [];
}
