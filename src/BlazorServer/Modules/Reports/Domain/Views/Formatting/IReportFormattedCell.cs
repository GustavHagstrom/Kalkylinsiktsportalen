namespace BlazorServer.Modules.Reports.Domain.Views;

public interface IReportFormattedCell
{
    CellLayout? CellLayout { get; set; }
    TextStyle? TextStyle { get; set; }
    ValueFormat? ValueFormat { get; set; }
}
