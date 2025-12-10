using System.Text.Json.Serialization;

namespace BlazorServer.Shared.DTOs.Bidcon;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum WorksheetStatusTypeDto
{
    None = 0,
    Bid = 1,
    Production = 3
}
