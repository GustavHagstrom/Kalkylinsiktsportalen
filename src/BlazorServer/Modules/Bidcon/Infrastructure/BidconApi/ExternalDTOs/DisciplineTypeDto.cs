using System.Text.Json.Serialization;

namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.ExternalDTOs;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DisciplineTypeDto
{
    None = 0,
    Bygg = 1,
    El = 2,
    Vs = 4,
    Anl = 8,
    Vent = 16,
    Gbr = 32,
    Nzl = 33,
    Aus = 34,
    Fin = 35,
    ElKraft = 36,
    Sgp = 37,
    Eur = 40,
    Ned = 41,
    Esp = 42
}
