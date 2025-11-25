using MongoDB.Bson;

namespace Data.MongoDb.Enteties.EstimationView;
public class UserView
{
    public ObjectId _id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string ViewId { get; set; } = string.Empty;
}
