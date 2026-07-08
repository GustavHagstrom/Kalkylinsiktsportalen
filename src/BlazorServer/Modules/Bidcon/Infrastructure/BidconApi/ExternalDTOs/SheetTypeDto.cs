using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SheetTypeDto
{
    None = 0,
    NetSheet = 1,
    TenderSheet = 2,
    PrelimsSheet = 3,
    SummarySheet = 14,
    LockedStagesSheet = 15,
    ReconciliationSheet = 18
}
