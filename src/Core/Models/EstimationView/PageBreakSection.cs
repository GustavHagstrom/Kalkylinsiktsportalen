using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public class PageBreakSection : ISection, IViewEntity
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public int Order { get; set; }
    [StringLength(50)]
    public string Name { get; set; } = "PageBreak";
    [StringLength(50)]
    public string ViewId { get; set; } = string.Empty;
    public View? View { get; set; }
    
}
