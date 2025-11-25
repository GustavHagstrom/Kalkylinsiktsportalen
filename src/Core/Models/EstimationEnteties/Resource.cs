namespace Core.Models.EstimationEnteties;
public class Resource
{
    public string Name { get; set; } = string.Empty;
    public double Quantity { get; set; }
    public double UnitCost { get; set; }
    public string? Account { get; set; }
    public string ResourceType { get; set; } = string.Empty;
}
