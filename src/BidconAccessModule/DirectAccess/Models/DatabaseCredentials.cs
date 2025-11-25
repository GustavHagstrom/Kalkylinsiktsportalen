namespace BidconAccessModule.DirectAccess.Models;

internal record DatabaseCredentials
    (
    string Server,
    string Database,
    string User,
    string Password,
    bool ServerAuthentication,
    bool UseDesktopBidconLink,
DateTime? LastUpdated = null);