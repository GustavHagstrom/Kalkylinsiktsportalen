namespace BlazorServer.Modules.Bidcon.ExternalDtos;

public class StructureCodeNode
{
    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<StructureCodeNode> Children { get; set; } = [];
}
