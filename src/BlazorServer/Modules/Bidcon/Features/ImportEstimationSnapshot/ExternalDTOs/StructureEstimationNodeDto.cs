namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot.ExternalDTOs;

public class StructureEstimationNodeDto
{
    public string Description { get; set; } = string.Empty;
    public List<StructureEstimationNodeDto> Children { get; set; } = [];
    public string Code { get; set; } = string.Empty;
    public string Remark { get; set; } = string.Empty;
    public RowTypeDto? RowType { get; set; }
    public int RowOrder { get; set; }
    public bool IsActive { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
}
