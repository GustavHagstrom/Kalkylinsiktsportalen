namespace BlazorServer.Modules.Reports.Domain.Views.DataSections;

public class RowDefinition
{
    public double HeightMm { get; private set; } = 8;

    public RowDefinition(double heightMm)
    {
        SetHeight(heightMm);
    }
    public void SetHeight(double heightMm)
    {
        if (heightMm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(heightMm), "Row height must be greater than zero.");
        }
        HeightMm = heightMm;
    }
};
