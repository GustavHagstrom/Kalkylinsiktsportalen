namespace BlazorServer.Modules.Bidcon.Features;

internal sealed class BidconFeatureSourceException(
    string code,
    string field,
    string message,
    Exception? innerException = null) : Exception(message, innerException)
{
    public string Code { get; } = code;

    public string Field { get; } = field;
}
