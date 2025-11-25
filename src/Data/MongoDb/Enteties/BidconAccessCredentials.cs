using Data.MongoDb.Enteties.Abstractions;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Data.MongoDb.Enteties;

public class BidconAccessCredentials  : ITenantEntety
{
    [BsonId]
    public string TenantId { get; set; } = string.Empty;
    public string Server { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    public string User { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool ServerAuthentication { get; set; }
    public bool UseDesktopBidconLink { get; set; }
    public int DesktopPort { get; set; }
    
}
