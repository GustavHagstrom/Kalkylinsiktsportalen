using BlazorServer.Modules.Access.Models;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Mappers;

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
