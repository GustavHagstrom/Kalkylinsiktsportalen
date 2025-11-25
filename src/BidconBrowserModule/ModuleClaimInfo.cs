using Core.Abstractions;
using Microsoft.Extensions.Localization;

namespace BidconBrowserModule;
internal class ModuleClaimInfo(IStringLocalizer<ModuleClaimInfo> Localizer) : IAccessClaimInfo
{
    public string Value => Constants.Authorization.AccessClaimValue;

    public string Name => Localizer["BidconUtforskaren"];

    public string Description => Localizer["Tillgång att bläddra i kalkylutforskaren"];
}
