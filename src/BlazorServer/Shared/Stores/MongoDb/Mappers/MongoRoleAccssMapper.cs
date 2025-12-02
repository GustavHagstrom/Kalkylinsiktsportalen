using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb.Enteties;

namespace BlazorServer.Shared.Stores.MongoDb.Mappers;

public static class MongoRoleAccessMapper
{
    public static MongoRoleAccess ToMongo(RoleAccess model)
    {
        return new MongoRoleAccess
        {
            RoleId = model.RoleId,
            OrganizationId = model.OrgId,
            AccessClaimValues = model.AccessClaimValues
        };
    }

    public static RoleAccess ToModel(MongoRoleAccess entety)
    {
        return new RoleAccess
        {
            RoleId = entety.RoleId,
            OrgId = entety.OrganizationId,
            AccessClaimValues = entety.AccessClaimValues
        };
    }
}
