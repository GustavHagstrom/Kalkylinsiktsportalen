using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.ExternalDtos;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ResourceTypeDto
{
    None = 0,

    Labor = 1,

    Officer = 2,

    Material = 3,

    Machine = 6,
    
    Information = 8,

    Subcontractor = 9,

    MaterialTender = 10,

    SubcontractorTender = 11,

    Adjustment = 99
}
