using Core.Models.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Data.EfCore;
using UserRightSettingsModule.Data.Abstractions;

namespace UserRightSettingsModule.Data.EfCore;
internal class SqlRoleStore(IDbContextFactory<ApplicationDbContext> dbContextFactory) : IRoleStore
{
    public Task UpsertRoleAsync(string tenantId, Role role)
    {
        using var context = dbContextFactory.CreateDbContext();
        var existingRole = context.Roles.FirstOrDefault(x => x.Id == role.Id);
        if (existingRole != null)
        {
            existingRole.Name = role.Name;
            existingRole.Description = role.Description;
            existingRole.TenantId = tenantId;
            context.Roles.Update(existingRole);
        }
        else
        {
            role.TenantId = tenantId;
            context.Roles.Add(role);
        }
        return context.SaveChangesAsync();
    }

    public Task DeleteRoleAsync(Role role)
    {
        using var context = dbContextFactory.CreateDbContext();
        context.Roles.Remove(role);
        return context.SaveChangesAsync();
    }

    public Task<Role?> GetRoleAsync(string roleId)
    {
        using var context = dbContextFactory.CreateDbContext();
        return context.Roles.FirstOrDefaultAsync(x => x.Id == roleId);
    }

    public Task<List<Role>> GetRolesAsync(string tenantId)
    {
        using var context = dbContextFactory.CreateDbContext();
        return context.Roles.Where(x => x.TenantId == tenantId).ToListAsync();
    }

    public async Task<List<Role>> GetUserRolesForUserAsync(string userId)
    {
        using var context = dbContextFactory.CreateDbContext();
        var result = await context.UserRoles.Include(x => x.Role)
            .Where(x => x.UserId == userId)
            .Select(x => x.Role)
            .ToListAsync();
        var roles = result?.Where(x => x is not null).Select(x => x!).ToList();
        return roles ?? [];

    }

    public Task<List<UserRole>> GetUserRolesForRoleAsync(string roleId)
    {
        using var context = dbContextFactory.CreateDbContext();
        return context.UserRoles.Where(x => x.RoleId == roleId).ToListAsync();
    }

    public Task CreateUserRoleAsync(string userId, Role role)
    {
        var userRole = new UserRole
        {
            RoleId = role.Id,
            UserId = userId
        };
        return CreateUserRoleAsync(userRole);
    }
    public Task CreateUserRoleAsync(UserRole userRole)
    {
        using var context = dbContextFactory.CreateDbContext();
        context.UserRoles.Add(userRole);
        return context.SaveChangesAsync();
    }
    public Task DeleteUserRoleAsync(string userId, Role role)
    {
        var userRole = new UserRole
        {
            RoleId = role.Id,
            UserId = userId
        };
        return DeleteUserRoleAsync(userRole);
    }
    public Task DeleteUserRoleAsync(UserRole userRole)
    {
        using var context = dbContextFactory.CreateDbContext();
        var existingUserRole = context.UserRoles.FirstOrDefault(x => x.RoleId == userRole.RoleId && x.UserId == userRole.UserId);
        if (existingUserRole != null)
        {
            context.UserRoles.Remove(existingUserRole);
        }
        return context.SaveChangesAsync();
    }
}
