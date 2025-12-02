using BlazorServer.Shared.Models;
using BlazorServer.Shared.Stores.MongoDb.Enteties;
using BlazorServer.Shared.Stores.MongoDb.Mappers;
using MongoDB.Driver;

namespace BlazorServer.Shared.Stores.MongoDb.Stores;

public class MongoAccessStore : IAccessStore
{
    private readonly IMongoCollection<MongoUserAccess> _userAccessCollection;
    private readonly IMongoCollection<MongoRoleAccess> _roleAccessCollection;

    public MongoAccessStore(IMongoCollection<MongoUserAccess> userAccessCollection, IMongoCollection<MongoRoleAccess> roleAccessCollection)
    {
        _userAccessCollection = userAccessCollection;
        _roleAccessCollection = roleAccessCollection;
    }
    public Task AddRoleAccess(string roleId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoRoleAccess>.Filter.Eq(ra => ra.RoleId, roleId) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
        var update = Builders<MongoRoleAccess>.Update.AddToSet(ra => ra.AccessClaimValues, accessClaimValue);
        return _roleAccessCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public Task AddUserAccess(string userId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var update = Builders<MongoUserAccess>.Update.AddToSet(ua => ua.AccessClaimValues, accessClaimValue);
        return _userAccessCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public Task DeleteRoleAccess(string roleId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoRoleAccess>.Filter.Eq(ra => ra.RoleId, roleId) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
        var update = Builders<MongoRoleAccess>.Update.Pull(ra => ra.AccessClaimValues, accessClaimValue);
        return _roleAccessCollection.UpdateOneAsync(filter, update);
    }

    public Task DeleteUserAccess(string userId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var update = Builders<MongoUserAccess>.Update.Pull(ua => ua.AccessClaimValues, accessClaimValue);
        return _userAccessCollection.UpdateOneAsync(filter, update);
    }

    public async Task<List<AccessClaim>> GetAccessClaimsForUser(string userId, IEnumerable<string> roleIds, string orgId)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var userAccess = await _userAccessCollection.Find(filter).FirstOrDefaultAsync();

        var accessClaimValues = new HashSet<string>(userAccess?.AccessClaimValues ?? Enumerable.Empty<string>());

        if (roleIds != null && roleIds.Any())
        {
            var roleFilter = Builders<MongoRoleAccess>.Filter.In(ra => ra.RoleId, roleIds) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
            var roleAccesses = await _roleAccessCollection.Find(roleFilter).ToListAsync();
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

    public async Task<RoleAccess> GetRoleAccess(string roleId, string orgId)
    {
        var filter = Builders<MongoRoleAccess>.Filter.Eq(ra => ra.RoleId, roleId) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
        var result = await _roleAccessCollection.Find(filter).FirstOrDefaultAsync();
        return MongoRoleAccessMapper.ToModel(result);
    }

    public async Task<UserAccess> GetUserAccess(string userId, string orgId)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var mongoUserAccess = await _userAccessCollection.Find(filter).FirstOrDefaultAsync();
        return MongoUserAccessMapper.ToModel(mongoUserAccess);
    }
}
