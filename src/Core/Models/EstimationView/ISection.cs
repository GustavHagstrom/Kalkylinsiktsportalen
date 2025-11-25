using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public interface ISection
{
    int Order { get; set; }
    [StringLength(50)]
    string Name { get; set; }
    [StringLength(450)]
    public string ViewId { get; set; }
    public View? View { get; set; }
}
