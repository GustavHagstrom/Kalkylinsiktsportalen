using Microsoft.AspNetCore.Components;

namespace BlazorServer.Shared.Abstractions;

public abstract class StyleComponent : ComponentBase
{
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? Style { get; set; }
}

