namespace BlazorServer.Modules.Bidcon.Infrastructure.Persistence;

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
            OrganizationId = entity.OrganizationId,
            ApiKey = entity.ApiKey,
            ApiSecret = entity.ApiSecret
        };
    }

    public static MongoBidconApiCredentials ToMongo(this BidconApiCredentials model)
    {
        return new MongoBidconApiCredentials
        {
            OrganizationId = model.OrganizationId,
            ApiKey = model.ApiKey,
            ApiSecret = model.ApiSecret
        };
    }
}
