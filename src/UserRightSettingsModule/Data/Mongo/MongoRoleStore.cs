using Core.Models.Authorization;
using UserRightSettingsModule.Data.Abstractions;
using MongoDB.Driver;
using MongoEnteties = Data.MongoDb.Enteties.Authorization;

namespace UserRightSettingsModule.Data.Mongo;
internal class MongoRoleStore
    (
    IMongoCollection<MongoEnteties.Role> roleCollection,
    IMongoCollection<MongoEnteties.UserRole> userRoleCollection
    )
    : IRoleStore
{
    public async Task CreateUserRoleAsync(string userId, Role role)
    {
        var userRole = await userRoleCollection.Find(ur => ur.UserId == userId && ur.RoleId == role.Id).FirstOrDefaultAsync();
        if (userRole is null)
        {
            userRole = new MongoEnteties.UserRole
            {
                UserId = userId,
                RoleId = role.Id
            };
            await userRoleCollection.InsertOneAsync(userRole);
        }
    }

    public Task CreateUserRoleAsync(UserRole userRole)
    {
        var mongoUserRole = new MongoEnteties.UserRole
        {
            UserId = userRole.UserId,
            RoleId = userRole.RoleId
        };
        return userRoleCollection.InsertOneAsync(mongoUserRole);
    }

    public Task DeleteRoleAsync(Role role)
    {
        return roleCollection.DeleteOneAsync(r => r.Id == role.Id);
    }

    public Task DeleteUserRoleAsync(string userId, Role role)
    {
        return userRoleCollection.DeleteOneAsync(ur => ur.UserId == userId && ur.RoleId == role.Id);
    }

    public Task DeleteUserRoleAsync(UserRole userRole)
    {
        return userRoleCollection.DeleteOneAsync(ur => ur.UserId == userRole.UserId && ur.RoleId == userRole.RoleId);
    }

    public async Task<Role?> GetRoleAsync(string roleId)
    {
        var mongoRole = await roleCollection.Find(r => r.Id == roleId).FirstOrDefaultAsync();
        if (mongoRole is null)
        {
            return null;
        }
        var role = new Role
        {
            Id = mongoRole.Id,
            TenantId = mongoRole.TenantId,
            Name = mongoRole.Name,
            Description = mongoRole.Description
        };
        role.RoleAccesses = mongoRole.AccessClaimValues.Select(acv => new RoleAccess
        {
            RoleId = mongoRole.Id,
            Role = role,
            AccessClaimValue = acv
        }).ToList();
        return role;
    }

    public async Task<List<Role>> GetRolesAsync(string tenantId)
    {
        var mongoRoles = await roleCollection.Find(r => r.TenantId == tenantId).ToListAsync();
        var roles = mongoRoles.Select(mr =>
        {
            var role = new Role
            {
                Id = mr.Id,
                TenantId = mr.TenantId,
                Name = mr.Name,
                Description = mr.Description
            };
            role.RoleAccesses = mr.AccessClaimValues.Select(acv => new RoleAccess
            {
                RoleId = mr.Id,
                Role = role,
                AccessClaimValue = acv
            }).ToList();
            return role;
        }).ToList();
        return roles;
    }

    public async Task<List<UserRole>> GetUserRolesForRoleAsync(string roleId)
    {
        var mongoUserRoles = await userRoleCollection.Find(ur => ur.RoleId == roleId).ToListAsync();
        var userRoles = mongoUserRoles.Select(mur => new UserRole
        {
            UserId = mur.UserId,
            RoleId = mur.RoleId
        }).ToList();
        return userRoles;
    }

    public async Task<List<Role>> GetUserRolesForUserAsync(string userId)
    {
        var mongoUserRoles = await userRoleCollection.Find(ur => ur.UserId == userId).ToListAsync();
        var roles = mongoUserRoles.Select(mur =>
        {
            var role = roleCollection.Find(r => r.Id == mur.RoleId).FirstOrDefault();
            if (role is null)
            {
                return null;
            }
            var r = new Role
            {
                Id = role.Id,
                TenantId = role.TenantId,
                Name = role.Name,
                Description = role.Description
            };
            r.RoleAccesses = role.AccessClaimValues.Select(acv => new RoleAccess
            {
                RoleId = role.Id,
                Role = r,
                AccessClaimValue = acv
            }).ToList();
            return r;
        }).Where(r => r is not null).Select(r => r!).ToList();
        return roles;
    }

    public Task UpsertRoleAsync(string tenantId, Role role)
    {
        var mongoRole = new MongoEnteties.Role
        {
            Id = role.Id,
            TenantId = tenantId,
            Name = role.Name,
            Description = role.Description,
            AccessClaimValues = role.RoleAccesses.Select(ra => ra.AccessClaimValue).ToList()
        };
        return roleCollection.ReplaceOneAsync(r => r.Id == role.Id, mongoRole, new ReplaceOptions { IsUpsert = true });
    }
}
