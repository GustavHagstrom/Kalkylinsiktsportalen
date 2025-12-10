using System.Text.Json.Serialization;

namespace BlazorServer.Shared.DTOs.Bidcon;

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
