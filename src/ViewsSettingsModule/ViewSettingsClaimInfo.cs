using Core.Abstractions;
using Microsoft.Extensions.Localization;

namespace ViewSettingsModule;
internal class ViewSettingsClaimInfo(IStringLocalizer<ViewSettingsClaimInfo> Localizer) : IAccessClaimInfo
{
    public string Value => Constants.Authorization.AccessClaimValue;

    public string Name => Localizer["Vy inställningar"];

    public string Description => Localizer["Skapa, ändra och ta bort kalkylvyer."];
}
