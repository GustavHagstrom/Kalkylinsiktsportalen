namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class RowDefinition
{
    public double HeightMm { get; set; }

    public double Height
    {
        get => HeightMm;
        set => HeightMm = value;
    }
}
