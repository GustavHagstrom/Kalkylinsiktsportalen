namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class RowDefinition
{
    public double HeightMm { get; set; } = 8;

    public RowDefinition(double heightMm)
    {
        HeightMm = heightMm;
    }
};
