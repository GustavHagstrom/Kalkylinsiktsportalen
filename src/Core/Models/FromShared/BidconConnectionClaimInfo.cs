using Core.Abstractions;
using Core.Constants;
using Microsoft.Extensions.Localization;

namespace Core.Models.FromShared;
public class BidconConnectionClaimInfo(IStringLocalizer<BidconConnectionClaimInfo> Localizer) : IAccessClaimInfo
{
    public string Value => SharedAccessClaimValues.BidconConnection;

    public string Name => Localizer["Bidconanslutning"];

    public string Description => Localizer["Ändra inställningar för anslutningen mot Bidcons databas."];
}
