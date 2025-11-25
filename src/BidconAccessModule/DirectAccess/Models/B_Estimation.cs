namespace BidconAccessModule.DirectAccess.Models;
internal class B_Estimation
{
    public Guid EstimationID { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public string? Place { get; set; }
    public string? HandlingOfficer { get; set; }
    public string? ConfirmationOfficer { get; set; }
    public bool IsLocked { get; set; }
    public int FolderNum { get; set; }
    public required string Currency { get; set; }
    public double ObjectFactor { get; set; }
    public double TenderTotal { get; set; }
    public required int TenderType { get; set; }
    public required int EstimationState { get; set; }
    public override string ToString()
    {
        return Description is null ? Name : $"{Name} - {Description}";
    }
}
