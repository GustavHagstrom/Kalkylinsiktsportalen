using MongoDB.Bson;

namespace Data.MongoDb.Enteties.EstimationView;
public class RoleView
{
    public ObjectId _id { get; set; }
    public string RoleId { get; set; } = string.Empty;
    public string ViewId { get; set; } = string.Empty;
}
