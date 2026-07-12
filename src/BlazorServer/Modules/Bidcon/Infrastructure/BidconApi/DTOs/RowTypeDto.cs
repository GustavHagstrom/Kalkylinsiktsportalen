using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
internal enum RowTypeDto
{
    None = 0,
    Group = 100,
    Part = 101,
    Posting = 102,
    MELayer = 103,
    DELayer = 104,
    PRLayer = 105,
    QuantityPosting = 106,
    LockedStages = 107,
    Text = 108,
    Invoice = 109
}
