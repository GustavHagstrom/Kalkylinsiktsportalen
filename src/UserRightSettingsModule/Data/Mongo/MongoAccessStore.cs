using Core.Models.Authorization;
using UserRightSettingsModule.Data.Abstractions;
using MongoDB.Driver;
using MongoEnteties = Data.MongoDb.Enteties.Authorization;

namespace UserRightSettingsModule.Data.Mongo;
internal class MongoAccessStore
    (
    IMongoCollection<MongoEnteties.Role> roleCollection, 
    IMongoCollection<MongoEnteties.UserAccess> userAccessCollection
    ) 
    : IAccessStore
{
    public async Task CreateRoleAccessAsync(RoleAccess roleAccess)
    {
        var role = await roleCollection.Find(r => r.Id == roleAccess.RoleId).FirstOrDefaultAsync();
        if (role is null)
        {
            role = new MongoEnteties.Role
            {
                Id = roleAccess.RoleId,
                AccessClaimValues = new List<string> { roleAccess.AccessClaimValue }
            };
            await roleCollection.InsertOneAsync(role);
        }
        else if(!role.AccessClaimValues.Contains(roleAccess.AccessClaimValue))
        {
            role.AccessClaimValues.Add(roleAccess.AccessClaimValue);
            await roleCollection.ReplaceOneAsync(r => r.Id == roleAccess.RoleId, role);
        }

    }

    public async Task CreateUserAccessAsync(UserAccess userAccess)
    {
        var mongoUserAccess = await userAccessCollection.Find(u => u.UserId == userAccess.UserId).FirstOrDefaultAsync();
        if (mongoUserAccess is null)
        {
            mongoUserAccess = new MongoEnteties.UserAccess
            {
                UserId = userAccess.UserId,
                AccessClaimValues = new List<string> { userAccess.AccessClaimValue }
            };
            await userAccessCollection.InsertOneAsync(mongoUserAccess);
        }
        else if (!mongoUserAccess.AccessClaimValues.Contains(userAccess.AccessClaimValue))
        {
            mongoUserAccess.AccessClaimValues.Add(userAccess.AccessClaimValue);
            await userAccessCollection.ReplaceOneAsync(u => u.UserId == userAccess.UserId, mongoUserAccess);
        }
    }

    public async Task DeleteRoleAccessAsync(RoleAccess roleAccess)
    {
        var role = await roleCollection.Find(r => r.Id == roleAccess.RoleId).FirstOrDefaultAsync();
        if (role is not null && role.AccessClaimValues.Contains(roleAccess.AccessClaimValue))
        {
            role.AccessClaimValues.Remove(roleAccess.AccessClaimValue);
            await roleCollection.ReplaceOneAsync(r => r.Id == roleAccess.RoleId, role);
        }
    }

    public async Task DeleteUserAccessAsync(UserAccess userAccess)
    {
        var mongoUserAccess = await userAccessCollection.Find(u => u.UserId == userAccess.UserId).FirstOrDefaultAsync();
        if (mongoUserAccess is not null && mongoUserAccess.AccessClaimValues.Contains(userAccess.AccessClaimValue))
        {
            mongoUserAccess.AccessClaimValues.Remove(userAccess.AccessClaimValue);
            await userAccessCollection.ReplaceOneAsync(u => u.UserId == userAccess.UserId, mongoUserAccess);
        }
    }

    public async Task<IEnumerable<RoleAccess>> GetRoleAccessClaimValuesAsync(string roleId)
    {
        var role = await roleCollection.Find(r => r.Id == roleId).FirstOrDefaultAsync();
        if (role is not null)
        {
            role.AccessClaimValues.Select(acv => new RoleAccess { RoleId = roleId, AccessClaimValue = acv });
        }
        return Enumerable.Empty<RoleAccess>();
    }

    public async Task<IEnumerable<UserAccess>> GetUserAccessClaimValuesAsync(string userId)
    {
        var mongoUserAccess = await userAccessCollection.Find(u => u.UserId == userId).FirstOrDefaultAsync();
        if (mongoUserAccess is not null)
        {
            return mongoUserAccess.AccessClaimValues.Select(acv => new UserAccess { UserId = userId, AccessClaimValue = acv });
        }
        return Enumerable.Empty<UserAccess>();
    }
}
