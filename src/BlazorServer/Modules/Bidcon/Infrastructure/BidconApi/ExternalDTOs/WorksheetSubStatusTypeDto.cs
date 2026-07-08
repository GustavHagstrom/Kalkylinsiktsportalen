using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorksheetSubStatusTypeDto
{
    None = 0,
    Bid = 1,
    Contract = 2,
    Production = 3
}
