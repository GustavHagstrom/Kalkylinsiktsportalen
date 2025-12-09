namespace BlazorServer.Shared.Stores.MongoDb.Mappers;

public class MongoBidconApiCredentialsMapper
{
    public static Models.Bidcon.BidconApiCredentials ToModel(Enteties.MongoBidconApiCredentials? entity)
    {
        if (entity is null) return new Models.Bidcon.BidconApiCredentials();

        return new Models.Bidcon.BidconApiCredentials
        {
            OrganizationId = entity.OrganizationId,
            ApiKey = entity.ApiKey,
            ApiSecret = entity.ApiSecret
        };
    }
    public static Enteties.MongoBidconApiCredentials ToMongo(Models.Bidcon.BidconApiCredentials model)
    {
        return new Enteties.MongoBidconApiCredentials
        {
            OrganizationId = model.OrganizationId,
            ApiKey = model.ApiKey,
            ApiSecret = model.ApiSecret
        };
    }
}
