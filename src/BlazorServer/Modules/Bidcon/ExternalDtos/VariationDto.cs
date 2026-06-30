namespace BlazorServer.Modules.Bidcon.ExternalDtos;

public class VariationDto
{
    public bool AffectsCostBudget { get; set; }
    public bool AffectsRevenueBudget { get; set; }
    public double? ApprovedAmount { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; }  = string.Empty;
    public string RevenueBudgetAccount { get; set; } = string.Empty;
}
