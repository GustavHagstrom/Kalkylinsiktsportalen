using Microsoft.AspNetCore.Components;

namespace BlazorServer.UiKit;

public abstract class StyleComponent : ComponentBase
{
    [Parameter] public string Class { get; set; } = string.Empty;
    [Parameter] public string Style { get; set; } = string.Empty;
}

