namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class RowDefinition
{
    public int RowIndex { get; set; }
    public double HeightMm { get; set; }

    public double Height
    {
        get => HeightMm;
        set => HeightMm = value;
    }
}
