using Core.Abstractions;
using Microsoft.Extensions.Localization;

namespace UserRightSettingsModule;
internal class ModuleAccessClaimInfo(IStringLocalizer<ModuleAccessClaimInfo> Localizer) : IAccessClaimInfo
{
    public string Value => Constants.AuthorizationConstants.AccessClaimValue;

    public string Name => Localizer["Rättigheter"];

    public string Description => Localizer["Skapa, ändra och ta bort användarrättigheter och roller."];
}