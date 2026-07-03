namespace BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;

public sealed class BidconEstimationDetailsClientException : Exception
{
    public BidconEstimationDetailsClientException(string code, string field, string message)
        : base(message)
    {
        Code = code;
        Field = field;
    }

    public string Code { get; }

    public string Field { get; }
}
