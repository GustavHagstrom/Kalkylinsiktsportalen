using BlazorServer.Modules.Reports.Domain.Views;

namespace BlazorServer.Modules.Reports.Features.ListViews;

public abstract record ListViewsResult;

public sealed record ViewsListed(IReadOnlyList<View> Views) : ListViewsResult;
