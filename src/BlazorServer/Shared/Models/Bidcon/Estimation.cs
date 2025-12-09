namespace BlazorServer.Shared.Models.Bidcon;

public class Estimation
{
    public string Id { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string BidconId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public string? Location { get; set; }
    public string? HandlingOfficer { get; set; }
    public string? ConfirmationOfficer { get; set; }
    public double? TenderTotal { get; set; }
    public override string ToString()
    {
        return Description is null ? Name : $"{Name} - {Description}";
    }
}
