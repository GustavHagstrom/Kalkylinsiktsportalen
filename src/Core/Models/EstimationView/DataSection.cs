using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;

public class DataSection : IViewEntity, ISection
{
    public DataSection()
    {
        AddColumn();
        AddRow();
    }
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string Name { get; set; } = "DataSection";
    public int Order { get; set; }
    public List<DataRow> Rows { get; set; } = new();
    public List<DataColumn> Columns { get; set; } = new();
    public bool IsFooter { get; set; } = false;
    public bool IsHeader { get; set; } = false;
    public List<DataCell> Cells { get; set; } = new();



    [StringLength(50)]
    public string ViewId { get; set; } = string.Empty;
    public View? View { get; set; }


    public void AddRow()
    {
        Rows.Add(new DataRow
        {
            DataSectionId = Id,
            DataSection = this,
            Order = Rows.Count + 1
        });
        foreach (var column in Columns)
        {
            AddCell(Rows.Count, column.Order);
        }
    }
    public void AddColumn()
    {
        Columns.Add(new DataColumn
        {
            DataSectionId = Id,
            DataSection = this,
            Order = Columns.Count + 1
        });
        foreach (var row in Rows)
        {
            AddCell(row.Order, Columns.Count);
        }
    }
    public void AddCell(int row, int column)
    {
        Cells.Add(new DataCell
        {
            DataSectionId = Id,
            DataSection = this,
            Row = row,
            Column = column
        });
    }
}
