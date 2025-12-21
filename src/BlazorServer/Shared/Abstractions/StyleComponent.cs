using Microsoft.AspNetCore.Components;

namespace BlazorServer.Shared.Abstractions;

public abstract class StyleComponent : ComponentBase
{
    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public string Style { get; set; } = string.Empty;
}

