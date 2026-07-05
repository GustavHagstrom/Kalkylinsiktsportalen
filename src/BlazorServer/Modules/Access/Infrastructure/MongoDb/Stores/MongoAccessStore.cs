using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Mappers;
using MongoDB.Driver;
using BlazorServer.Modules.Access.Domain;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;

public class MongoAccessStore : IFunctionalAccessGrantStore
{
    private readonly IMongoCollection<MongoFunctionalAccessGrant> _accessGrants;

    public MongoAccessStore(IMongoCollection<MongoFunctionalAccessGrant> accessGrants)
    {
        _accessGrants = accessGrants;
    }

    public Task AddAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId, string accessClaimValue)
    {
        var update = Builders<MongoFunctionalAccessGrant>.Update
            .SetOnInsert(grant => grant.TargetType, target.Type)
            .SetOnInsert(grant => grant.TargetId, target.Id)
            .SetOnInsert(grant => grant.OrganizationId, orgId)
            .AddToSet(grant => grant.AccessClaimValues, accessClaimValue);

        return _accessGrants.UpdateOneAsync(FilterFor(target, orgId), update, new UpdateOptions { IsUpsert = true });
    }

    public Task DeleteAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId, string accessClaimValue)
    {
        var update = Builders<MongoFunctionalAccessGrant>.Update.Pull(grant => grant.AccessClaimValues, accessClaimValue);
        return _accessGrants.UpdateOneAsync(FilterFor(target, orgId), update);
    }

    public async Task<List<AccessClaim>> GetAccessClaimsForUserAsync(string userId, IEnumerable<string> roleIds, string orgId)
    {
        var userAccess = await GetAccessGrantAsync(FunctionalAccessGrantTarget.User(userId), orgId);

        var accessClaimValues = new HashSet<string>(userAccess?.AccessClaimValues ?? Enumerable.Empty<string>());

        if (roleIds != null && roleIds.Any())
        {
            var roleAccessTasks = roleIds.Select(roleId => GetLazyMongoAccessGrantAsync(
                FunctionalAccessGrantTarget.Role(roleId),
                orgId));
            var roleAccesses = await Task.WhenAll(roleAccessTasks);
            foreach (var roleAccess in roleAccesses)
            {
                foreach (var claimValue in roleAccess.AccessClaimValues)
                {
                    accessClaimValues.Add(claimValue);
                }
            }
        }
        return accessClaimValues.Select(x => new AccessClaim(x)).ToList();
    }

    public async Task<FunctionalAccessGrant> GetAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId)
    {
        var result = await GetLazyMongoAccessGrantAsync(target, orgId);
        return MongoFunctionalAccessGrantMapper.ToModel(result);
    }

    async Task<MongoFunctionalAccessGrant> GetLazyMongoAccessGrantAsync(FunctionalAccessGrantTarget target, string orgId)
    {
        var grant = await _accessGrants.Find(FilterFor(target, orgId)).FirstOrDefaultAsync();
        if (grant != null)
        {
            return grant;
        }

        var newGrant = new MongoFunctionalAccessGrant
        {
            TargetType = target.Type,
            TargetId = target.Id,
            OrganizationId = orgId,
            AccessClaimValues = new List<string>()
        };
        await _accessGrants.InsertOneAsync(newGrant);
        return newGrant;
    }

    static FilterDefinition<MongoFunctionalAccessGrant> FilterFor(FunctionalAccessGrantTarget target, string orgId)
    {
        return Builders<MongoFunctionalAccessGrant>.Filter.Eq(grant => grant.TargetType, target.Type) &
            Builders<MongoFunctionalAccessGrant>.Filter.Eq(grant => grant.TargetId, target.Id) &
            Builders<MongoFunctionalAccessGrant>.Filter.Eq(grant => grant.OrganizationId, orgId);
    }
}
