using Data.MongoDb.Enteties.Abstractions;
using Data.MongoDb.Enteties.EstimationView;
using System.ComponentModel.DataAnnotations;

namespace Data.MongoDb.Enteties;

public class UserViewTemplate : IUserEntety
{
    [StringLength(450)]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [StringLength(450)]
    public string EstimationViewTemplateId { get; set; } = string.Empty;
    public View? EstimationViewTemplate { get; set; }
    public string UserId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
