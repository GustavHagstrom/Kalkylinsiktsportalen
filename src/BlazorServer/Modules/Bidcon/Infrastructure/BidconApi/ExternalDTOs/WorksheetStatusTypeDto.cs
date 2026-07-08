using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorksheetStatusTypeDto
{
    None = 0,
    Bid = 1,
    Production = 3
}
