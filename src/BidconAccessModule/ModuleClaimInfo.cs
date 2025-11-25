using Core.Abstractions;
using Microsoft.Extensions.Localization;

namespace BidconAccessModule;
internal class ModuleClaimInfo(IStringLocalizer<ModuleClaimInfo> Localizer) : IAccessClaimInfo
{
    public string Value => Constants.Authorization.AccessClaimValue;

    public string Name => Localizer["BidconAnslutning"];

    public string Description => Localizer["Tillåts att modifiera Bidcon-anslutning"];
}
