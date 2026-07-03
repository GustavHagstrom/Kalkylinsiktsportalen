using BlazorServer.Modules.Access.Features.FunctionalAccessGrants;
using BlazorServer.Modules.Access.Models;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Entities;
using BlazorServer.Modules.Access.Infrastructure.MongoDb.Mappers;
using MongoDB.Driver;

namespace BlazorServer.Modules.Access.Infrastructure.MongoDb.Stores;

public class MongoAccessStore : IFunctionalAccessGrantStore
{
    private readonly IMongoCollection<MongoUserAccess> _userAccessCollection;
    private readonly IMongoCollection<MongoRoleAccess> _roleAccessCollection;

    public MongoAccessStore(IMongoCollection<MongoUserAccess> userAccessCollection, IMongoCollection<MongoRoleAccess> roleAccessCollection)
    {
        _userAccessCollection = userAccessCollection;
        _roleAccessCollection = roleAccessCollection;
    }
    public Task AddRoleAccessAsync(string roleId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoRoleAccess>.Filter.Eq(ra => ra.RoleId, roleId) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
        var update = Builders<MongoRoleAccess>.Update.AddToSet(ra => ra.AccessClaimValues, accessClaimValue);
        return _roleAccessCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public Task AddUserAccessAsync(string userId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var update = Builders<MongoUserAccess>.Update.AddToSet(ua => ua.AccessClaimValues, accessClaimValue);
        return _userAccessCollection.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
    }

    public Task DeleteRoleAccessAsync(string roleId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoRoleAccess>.Filter.Eq(ra => ra.RoleId, roleId) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
        var update = Builders<MongoRoleAccess>.Update.Pull(ra => ra.AccessClaimValues, accessClaimValue);
        return _roleAccessCollection.UpdateOneAsync(filter, update);
    }

    public Task DeleteUserAccessAsync(string userId, string orgId, string accessClaimValue)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var update = Builders<MongoUserAccess>.Update.Pull(ua => ua.AccessClaimValues, accessClaimValue);
        return _userAccessCollection.UpdateOneAsync(filter, update);
    }

    public async Task<List<AccessClaim>> GetAccessClaimsForUserAsync(string userId, IEnumerable<string> roleIds, string orgId)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var userAccess = await GetUserAccessAsync(userId, orgId);

        var accessClaimValues = new HashSet<string>(userAccess?.AccessClaimValues ?? Enumerable.Empty<string>());

        if (roleIds != null && roleIds.Any())
        {
            var roleFilter = Builders<MongoRoleAccess>.Filter.In(ra => ra.RoleId, roleIds) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);


            var roleAccessTasks = roleIds.Select(roleId => GetLazyMongoRoleAccessAsync(roleId, orgId));
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

    public async Task<RoleAccess> GetRoleAccessAsync(string roleId, string orgId)
    {
        var result = await GetLazyMongoRoleAccessAsync(roleId, orgId);
        return MongoRoleAccessMapper.ToModel(result);
    }

    public async Task<UserAccess> GetUserAccessAsync(string userId, string orgId)
    {
        var mongoUserAccess = await GetLazyMongoUserAccessAsync(userId, orgId);
        return MongoUserAccessMapper.ToModel(mongoUserAccess);
    }

    async Task<MongoUserAccess> GetLazyMongoUserAccessAsync(string userId, string orgId)
    {
        var filter = Builders<MongoUserAccess>.Filter.Eq(ua => ua.UserId, userId) & Builders<MongoUserAccess>.Filter.Eq(ua => ua.OrganizationId, orgId);
        var user = await _userAccessCollection.Find(filter).FirstOrDefaultAsync();
        if (user != null)
        {
            return user;
        }
        var newUserAccess = new MongoUserAccess
        {
            UserId = userId,
            OrganizationId = orgId,
            AccessClaimValues = new List<string>()
        };
        await _userAccessCollection.InsertOneAsync(newUserAccess);
        return newUserAccess;
    }
    async Task<MongoRoleAccess> GetLazyMongoRoleAccessAsync(string roleId, string orgId)
    {
        var filter = Builders<MongoRoleAccess>.Filter.Eq(ra => ra.RoleId, roleId) & Builders<MongoRoleAccess>.Filter.Eq(ra => ra.OrganizationId, orgId);
        var role = await _roleAccessCollection.Find(filter).FirstOrDefaultAsync();
        if(role != null)
        {
            return role;
        }
        var newRoleAccess = new MongoRoleAccess
        {
            RoleId = roleId,
            OrganizationId = orgId,
            AccessClaimValues = new List<string>()
        };
        await _roleAccessCollection.InsertOneAsync(newRoleAccess);
        return newRoleAccess;
    }
}
