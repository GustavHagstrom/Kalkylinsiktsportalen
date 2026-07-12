using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum WorksheetStatusTypeDto
{
    None = 0,
    Bid = 1,
    Production = 3
}
