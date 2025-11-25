namespace Data.MongoDb.Enteties.EstimationView;

public class DataSection
{
    public string Name { get; set; } = "DataSection";
    public int Order { get; set; }
    public List<DataRow> Rows { get; set; } = new();
    public List<DataColumn> Columns { get; set; } = new();
    public bool IsFooter { get; set; } = false;
    public bool IsHeader { get; set; } = false;
    public List<DataCell> Cells { get; set; } = new();

}
