using BidconBrowserModule.Models;
using System.Security.Claims;

namespace BidconBrowserModule.Services;
internal interface IBidconBrowserAccesService
{
    Task<TreeItem> GetTreeItemRootAsync(string tenantId);
}
