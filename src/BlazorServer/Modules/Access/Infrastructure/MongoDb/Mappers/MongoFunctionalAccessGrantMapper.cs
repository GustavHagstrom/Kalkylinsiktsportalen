using BlazorServer.Identity;
using BlazorServer.Modules.Access.Domain;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Mappers;

public static class MongoFunctionalAccessGrantMapper
{
    public static MongoFunctionalAccessGrant ToMongo(FunctionalAccessGrant model)
    {
        return new MongoFunctionalAccessGrant
        {
            TargetType = model.Target.Type,
            TargetId = model.Target.Id,
            OrganizationId = model.OrganizationId.Value,
            AccessClaimValues = model.AccessClaimValues.ToList()
        };
    }

    public static FunctionalAccessGrant ToModel(MongoFunctionalAccessGrant entity)
    {
        return new FunctionalAccessGrant(
            new FunctionalAccessGrantTarget(entity.TargetType, entity.TargetId),
            new OrganizationId(entity.OrganizationId),
            entity.AccessClaimValues);
    }
}
