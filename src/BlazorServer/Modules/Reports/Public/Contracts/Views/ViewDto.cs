using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection;
using BlazorServer.Modules.Reports.Public.Contracts.Views.SheetSection;

namespace BlazorServer.Modules.Reports.Public.Contracts.Views;

public class ViewDto
{
    public ViewId Id { get; set; } = ViewId.New();
    public OrganizationId OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataSectionDto> DataSections { get; set; } = new();
    public List<SheetSectionDto> SheetSections { get; set; } = new();
    public List<PageBreakSectionDto> PageBreakSections { get; set; } = new();
}
