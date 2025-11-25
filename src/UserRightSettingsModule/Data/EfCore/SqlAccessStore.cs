using Core.Models.Authorization;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Data.EfCore;
using UserRightSettingsModule.Data.Abstractions;

namespace UserRightSettingsModule.Data.EfCore;
internal class SqlAccessStore : IAccessStore
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public SqlAccessStore(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public Task CreateRoleAccessAsync(RoleAccess roleAccess)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.RoleAccess.Add(roleAccess);
        return context.SaveChangesAsync();
    }

    public Task CreateUserAccessAsync(UserAccess userAccess)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.UserAccess.Add(userAccess);
        return context.SaveChangesAsync();
    }

    public Task DeleteRoleAccessAsync(RoleAccess roleAccess)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.RoleAccess.Remove(roleAccess);
        return context.SaveChangesAsync();
    }

    public Task DeleteUserAccessAsync(UserAccess userAccess)
    {
        using var context = _dbContextFactory.CreateDbContext();
        context.UserAccess.Remove(userAccess);
        return context.SaveChangesAsync();
    }

    public async Task<IEnumerable<RoleAccess>> GetRoleAccessClaimValuesAsync(string roleId)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var roleAccesses = await context.RoleAccess
            .Where(ra => ra.RoleId == roleId)
            .ToListAsync();
        return roleAccesses ?? new();
    }

    public Task<Role?> GetRoleAsync(string roleId)
    {
        using var context = _dbContextFactory.CreateDbContext();
        return context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
    }

    public async Task<IEnumerable<Role>> GetRolesAsync(ClaimsPrincipal user)
    {
        var tenantId = user.GetTenantId();
        if (tenantId == null)
        {
            throw new ArgumentNullException(nameof(tenantId));
        }
        using var context = _dbContextFactory.CreateDbContext();
        var roles = await context.Roles.Where(x => x.TenantId == tenantId).ToListAsync();
        return roles;
    }

    public async Task<IEnumerable<UserAccess>> GetUserAccessClaimValuesAsync(string userId)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var userAccesses = await context.UserAccess
            .Where(ua => ua.UserId == userId)
            .ToListAsync();
        return userAccesses ?? new();
    }
}
