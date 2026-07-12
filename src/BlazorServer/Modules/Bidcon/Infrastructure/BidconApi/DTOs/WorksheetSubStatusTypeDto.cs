using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum WorksheetSubStatusTypeDto
{
    None = 0,
    Bid = 1,
    Contract = 2,
    Production = 3
}
