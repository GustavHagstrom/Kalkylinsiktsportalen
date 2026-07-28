namespace BlazorServer.Modules.Bidcon.Features;

internal sealed class BidconFeatureSourceException(
    string code,
    string field,
    string message) : Exception(message)
{
    public string Code { get; } = code;

    public string Field { get; } = field;
}
