using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb.Enteties;

namespace BlazorServer.Shared.Stores.MongoDb.Mappers;

public static class MongoUserAccessMapper
{
    public static MongoUserAccess ToMongo(UserAccess model)
    {
        return new MongoUserAccess
        {
            UserId = model.UserId,
            OrganizationId = model.OrganizationId,
            AccessClaimValues = model.AccessClaimValues
        };
    }
    public static UserAccess ToModel(MongoUserAccess entety)
    {
        return new UserAccess
        {
            UserId = entety.UserId,
            OrganizationId = entety.OrganizationId,
            AccessClaimValues = entety.AccessClaimValues
        };
    }
}
