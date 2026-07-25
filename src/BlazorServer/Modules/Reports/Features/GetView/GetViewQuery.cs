using BlazorServer.Kernel.CQRS;
using BlazorServer.Modules.Reports.Public;

namespace BlazorServer.Modules.Reports.Features.GetView;

public sealed record GetViewQuery(ViewId ViewId) : IQuery<GetViewResult>;
