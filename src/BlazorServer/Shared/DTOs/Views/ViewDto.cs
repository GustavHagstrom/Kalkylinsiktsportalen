using BlazorServer.Shared.DTOs.Views.DataSection;
using BlazorServer.Shared.DTOs.Views.SheetSection;

namespace BlazorServer.Shared.DTOs.Views;

public class ViewDto
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string OrganizationId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<DataSectionDto> DataSections { get; set; } = new();
    public List<SheetSectionDto> SheetSections { get; set; } = new();
    public List<PageBreakSection> PageBreakSections { get; set; } = new();
}
