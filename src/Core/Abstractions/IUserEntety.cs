using System.ComponentModel.DataAnnotations;

namespace Core.Abstractions;
public interface IUserEntety
{
    [StringLength(50)]
    public string UserId { get; set; }
}
