namespace BlazorServer.Modules.Reports.Domain.Views;

public interface IViewSection
{
    SectionId SectionId { get; }
    IViewSection Clone();
    PageBreakBehavior PageBreakBehavior { get; set; }
}
