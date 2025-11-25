using Core.Models;
using System.Security.Claims;

namespace BidconAccessModule.DirectAccess.Data.Abstractions;
public interface IDirectCredentialsStore
{
    Task<BidconAccessCredentials?> GetAsync(string tenantId);
    Task UpsertAsync(string tenantId, BidconAccessCredentials credentialsDto);
}