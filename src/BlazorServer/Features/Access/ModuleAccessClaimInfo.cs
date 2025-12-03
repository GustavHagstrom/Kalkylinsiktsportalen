using BlazorServer.Shared.Abstractions;
using Microsoft.Extensions.Localization;

namespace BlazorServer.Features.Access;
internal class ModuleAccessClaimInfo(IStringLocalizer<ModuleAccessClaimInfo> Localizer) : IAccessClaimInfo
{
    public string Value => FeatureConstants.Authorization.AccessClaimValue;

    public string Name => Localizer["Rättigheter"];

    public string Description => Localizer["Skapa, ändra och ta bort användarrättigheter och roller."];
}