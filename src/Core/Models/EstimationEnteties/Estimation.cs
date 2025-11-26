using Core.Abstractions;

namespace Core.Models.EstimationEnteties;
public class Estimation : ITenantEntety
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TenantId { get; set; } = string.Empty;
    public required string BidconId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public string? Location { get; set; }
    public string? HandlingOfficer { get; set; }
    public string? ConfirmationOfficer { get; set; }
    public double? TenderTotal { get; set; }
    public SheetItem? NetSheet { get; set; }
    public List<Resource> Resources { get; set; } = new();
    public List<QuantityPointer> QuantityChanges { get; set; } = new();
    public IEnumerable<IUndoRedoItem> ChangesInOrder => QuantityChanges.Cast<IUndoRedoItem>();

    //Add support for this later?
    //public ICollection<ISheetItem> LockedStages { get; set; } = new List<ISheetItem>();

    //Add support for ATA later?
    public override string ToString()
    {
        return Description is null ? Name : $"{Name} - {Description}";
    }
}
