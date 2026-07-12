namespace BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Clients;

internal sealed class BidconEstimationClientException : Exception
{
    public BidconEstimationClientException(string code, string field, string message)
        : base(message)
    {
        Code = code;
        Field = field;
    }

    public string Code { get; }

    public string Field { get; }
}
