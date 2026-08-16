namespace BlazorServer.Modules.Reports.Domain.Views;

public interface IViewSection
{
    SectionId Id { get; }
    IViewSection Clone();
    
}
