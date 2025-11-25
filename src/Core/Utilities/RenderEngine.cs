using Microsoft.AspNetCore.Components;

namespace Core.Utilities;
public static class RenderEngine
{
    public static RenderFragment Render(Type componentType, params KeyValuePair<string, object>[] parameters)
    {
        return builder =>
        {
            builder.OpenComponent(0, componentType);
            builder.AddMultipleAttributes(1, parameters);
            builder.CloseComponent();
        };
    }
    public static RenderFragment Render<T>(params KeyValuePair<string, object>[] parameters) where T : IComponent
    {
        return Render(typeof(T), parameters);
    }
}
