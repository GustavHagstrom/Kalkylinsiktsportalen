using System.ComponentModel.DataAnnotations;

namespace Data.MongoDb.Enteties.EstimationEnteties;
public class UnitOfChange
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string EstimationId { get; set; } = string.Empty;
    public Estimation? Estimation { get; set; }
    public IEnumerable<SheetItemChange> SheetItemChanges { get; set; } = new List<SheetItemChange>();
}
