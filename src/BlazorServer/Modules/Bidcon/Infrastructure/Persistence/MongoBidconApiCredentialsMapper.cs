namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

using BlazorServer.Identity;

public static class MongoBidconApiCredentialsMapper
{
    public static BidconApiCredentials? ToModel(this MongoBidconApiCredentials? entity)
    {
        if (entity is null)
        {
            return null;
        }

        return new BidconApiCredentials
        {
            OrganizationId = new OrganizationId(entity.OrganizationId),
            ApiKey = entity.ApiKey,
            ApiSecret = entity.ApiSecret
        };
    }

    public static MongoBidconApiCredentials ToMongo(this BidconApiCredentials model)
    {
        return new MongoBidconApiCredentials
        {
            OrganizationId = model.OrganizationId.Value,
            ApiKey = model.ApiKey,
            ApiSecret = model.ApiSecret
        };
    }
}
