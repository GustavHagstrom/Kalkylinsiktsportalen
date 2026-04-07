namespace BlazorServer.Shared.Models.Bidcon;

public class Estimation
{
    public string Id { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string BidconId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Customer { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string HandlingOfficer { get; set; } = string.Empty;
    public string ConfirmationOfficer { get; set; } = string.Empty;
    public double? TenderTotal { get; set; }
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Description) ? Name : $"{Name} - {Description}";
    }
}
