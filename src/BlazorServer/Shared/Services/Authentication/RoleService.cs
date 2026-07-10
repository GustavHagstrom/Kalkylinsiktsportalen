using BlazorServer.Shared.Constants;
using BlazorServer.Shared.Models;
using Microsoft.Graph;

namespace BlazorServer.Shared.Services.Authentication;

public class RoleService : IRoleService
{
    private readonly GraphServiceClient _graphClient;
    private readonly IUserService _userService;
    private readonly IAdminRoleCache _cache;

    public RoleService(GraphServiceClient graphClient, IUserService userService, IAdminRoleCache claimsCache)
    {
        _graphClient = graphClient;
        _userService = userService;
        _cache = claimsCache;
    }

    public async Task<bool> IsAdminAsync(string userId, string orgId)
    {
        if(_cache.TryGetIsAdminRole(userId, orgId, out bool isAdmin))
        {
            return isAdmin;
        }
        else
        {
            var roleResult = await _graphClient.Users[userId].MemberOf.GetAsync();
            var adminRole = roleResult?.Value?
            .Where(r => r is Microsoft.Graph.Models.DirectoryRole)
            .Select(dr => dr as Microsoft.Graph.Models.DirectoryRole)
            .FirstOrDefault(dr => dr?.RoleTemplateId == AuthConstants.AdminRoleClaimValue);
            if (adminRole != null)
            {
                _cache.SetIsAdminRole(userId, orgId);
                return true;
            }
            else
            {
                _cache.ClearIsAdminRole(userId, orgId);
            }
        }
        return false;
    }

    public async Task<Role?> GetRoleAsync(string roleId)
    {
        var group = await _graphClient.Groups[roleId].GetAsync();
        return group == null ? null : new Role
        {
            Id = group.Id ?? string.Empty,
            Name = group.DisplayName ?? string.Empty,
            Description = group.Description ?? string.Empty 
        };
    }

    public async Task<List<Role>> GetRolesAsync(string orgId)
    {
        var groupsResponse = await _graphClient.Groups.GetAsync();
        var groups = groupsResponse?.Value;
        var roles = groups?.Select(g => new Role
        {
            Id = g.Id!,
            Name = g.DisplayName!,
            Description = g.Description!
        }).ToList() ?? new List<Role>();
        return roles;
    }

    public async Task<List<Role>> GetRolesForUserAsync(string userId, string orgId)
    {
        var response = await _graphClient.Users[userId].MemberOf.GetAsync();
        if (response?.Value is null) return [];
        List<Role> roles = [];
        foreach (var item in response.Value)
        {
            if(item is Microsoft.Graph.Models.Group group)
            {
                roles.Add(new Role
                {
                    Id = group.Id ?? string.Empty,
                    Name = group.DisplayName ?? string.Empty,
                    Description = group.Description ?? string.Empty
                });
            }
        }
        return roles;
    }

    public async Task<List<User>> GetUsersInRoleAsync(string roleId, string orgId)
    {
        var members = await _graphClient.Groups[roleId].Members.GetAsync();
        List<string> userIds = [];
        foreach (var member in members?.Value ?? [])
        {
            if (member is Microsoft.Graph.Models.User user && user.Id is not null)
            {
                userIds.Add(user.Id);
            }
        }
        var allUsers = await _userService.GetUsersAsync(orgId);
        return allUsers.Where(u => userIds.Contains(u.Id)).ToList();
    }

}
