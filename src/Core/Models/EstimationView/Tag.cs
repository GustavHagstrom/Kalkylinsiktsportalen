using Core.Enums.ViewTemplate;
using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;

public class Tag : IViewEntity
{
    [StringLength(50)]
    public string Id { get; set; } = Guid.NewGuid().ToString();
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    [StringLength(50)]
    public string Value { get; set; } = string.Empty;
    public TagType Type { get; set; }
    [StringLength(50)]
    public string ViewId { get; set; } = string.Empty;
    public View? View { get; set; }

}
