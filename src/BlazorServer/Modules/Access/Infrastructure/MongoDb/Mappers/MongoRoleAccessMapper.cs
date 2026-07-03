using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Mappers;

public static class MongoRoleAccessMapper
{
    public static MongoRoleAccess ToMongo(RoleAccess model)
    {
        return new MongoRoleAccess
        {
            RoleId = model.RoleId,
            OrganizationId = model.OrganizationId,
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
