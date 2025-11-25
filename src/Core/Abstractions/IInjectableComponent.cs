using Core.Utilities;
using Microsoft.AspNetCore.Components;

namespace Core.Abstractions;
public interface IInjectableComponent
{
    RenderFragment Render()
    {
        return RenderEngine.Render(GetType());
    }
}
