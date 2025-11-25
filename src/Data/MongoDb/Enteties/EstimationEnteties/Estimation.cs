using Data.MongoDb.Enteties.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace Data.MongoDb.Enteties.EstimationEnteties;
public class Estimation : ITenantEntety
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string TenantId { get; set; } = string.Empty;
    [StringLength(50)]
    public required string BidconId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Customer { get; set; }
    public string? Place { get; set; }
    public string? HandlingOfficer { get; set; }
    public string? ConfirmationOfficer { get; set; }
    public double? TenderTotal { get; set; }
    [StringLength(50)]
    public string NetSheetId { get; set; } = string.Empty;
    public SheetItem? NetSheet { get; set; }
    

    //Add support for this later?
    //public ICollection<ISheetItem> LockedStages { get; set; } = new List<ISheetItem>();

    //Add support for ATA later?
}
