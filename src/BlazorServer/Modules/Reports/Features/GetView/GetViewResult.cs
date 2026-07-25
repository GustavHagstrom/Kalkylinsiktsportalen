using BlazorServer.Modules.Reports.Public.Contracts.Views;

namespace BlazorServer.Modules.Reports.Features.GetView;

public abstract record GetViewResult;

public sealed record ViewLoaded(ViewDto? View) : GetViewResult;
