using System.Security.Claims;

namespace BidconAccessModule.DirectAccess.Data.Abstractions;

public interface IDbConnectionStringStore

{
    Task<string> BuildAsync(string tenantId);
}