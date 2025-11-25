using Core.Extensions;
using Core.Models.FromShared;
using Data.Abstractions;
using Data.MongoDb.Enteties.Authorization;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Security.Claims;

namespace Data.MongoDb.Stores;
public class MongoAccessClaimStore
    (
    IMongoCollection<Role> roleCollection,
    IMongoCollection<UserAccess> userAccessCollection,
    IMongoCollection<UserRole> userRoleCollection,
    ILogger<MongoAccessClaimStore> logger
    ) 
    : IAccessClaimStore
{

    public Task<IEnumerable<AccessClaim>> GetAccessClaims(ClaimsPrincipal? userClaims)
    {
        var userId = userClaims?.GetUserId();
        if (userId == null)
        {
            logger.LogWarning("User id is null");
            return Task.FromResult(Enumerable.Empty<AccessClaim>());
        }

        return GetAccessClaims(userId);
    }

    public async Task<IEnumerable<AccessClaim>> GetAccessClaims(string userId)
    {
        var userRoles = await userRoleCollection.Find(x => x.UserId == userId).ToListAsync();
        var userAccess = await userAccessCollection.Find(x => x.UserId == userId).FirstOrDefaultAsync();
        var accessClaims = new List<AccessClaim>();
        var accessValues = new List<string>();
        var aClaims = new Dictionary<string, AccessClaim>();
        foreach (var userRole in userRoles)
        {
            var role = await roleCollection.Find(x => x.Id == userRole.RoleId).FirstOrDefaultAsync();
            if (role == null)
            {
                logger.LogWarning("Role not found");
                continue;
            }
            var values = role.AccessClaimValues.Where(x => accessValues.Contains(x) == false).ToList();
            accessClaims.AddRange(values.Select(ac => new AccessClaim(ac)));
            accessValues.AddRange(values);
        }
        if (userAccess is not null)
        {
            var values = userAccess.AccessClaimValues.Where(x => accessValues.Contains(x) == false).ToList();
            accessClaims.AddRange(values.Select(x => new AccessClaim(x)));
        }
        
        return accessClaims;
    }
}
