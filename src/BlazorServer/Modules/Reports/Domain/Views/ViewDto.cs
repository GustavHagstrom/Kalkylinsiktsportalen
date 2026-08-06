using BlazorServer.Identity;
using BlazorServer.Modules.Reports.Public;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;
using BlazorServer.Modules.Reports.Domain.Views.SheetSection;

namespace BlazorServer.Modules.Reports.Domain.Views;

public class ViewDto
{
    public ViewId Id { get; set; } = ViewId.New();
    public OrganizationId OrganizationId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataSectionDto> DataSections { get; set; } = new();
    public List<SheetSectionDto> SheetSections { get; set; } = new();
}
