using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public class DataCell : IViewEntity
{
    public DataCell()
    {
        CellFormat = new DataCellFormat
        {
            DataCellId = Id,
            DataCell = this
        };
    }
    
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Row { get; set; }
    public int Column { get; set; }
    [StringLength(500)]
    public string Formula { get; set; } = string.Empty;
    [StringLength(50)]
    public string DataSectionId { get; set; } = string.Empty;
    public DataSection? DataSection { get; set; }
    public DataCellFormat? CellFormat { get; set; }
}