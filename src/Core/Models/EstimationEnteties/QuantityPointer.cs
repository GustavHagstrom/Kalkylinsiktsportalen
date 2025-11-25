namespace Core.Models.EstimationEnteties;
public class QuantityPointer : IUndoRedoItem
{
    public int Order { get; set; }
    public string FromSheetItemId { get; set; } = string.Empty;
    public string ToSheetItemId { get; set; } = string.Empty;
    
}
