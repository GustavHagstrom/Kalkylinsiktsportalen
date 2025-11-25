using System.ComponentModel.DataAnnotations;

namespace Core.Models.EstimationView;
public class UserView
{
    [StringLength(50)]
    public string UserId { get; set; } = string.Empty;
    [StringLength(50)]
    public string ViewId { get; set; } = string.Empty;
    public View? View { get; set; }
}
