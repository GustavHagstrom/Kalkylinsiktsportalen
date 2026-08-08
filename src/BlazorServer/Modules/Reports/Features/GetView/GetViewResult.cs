using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Features.GetView;

public abstract record GetViewResult;

public sealed record ViewLoaded(View? View) : GetViewResult;
