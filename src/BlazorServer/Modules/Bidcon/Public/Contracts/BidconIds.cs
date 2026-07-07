namespace BlazorServer.Modules.Bidcon.Public.Contracts;

public readonly record struct BidconEstimationId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}

public readonly record struct EstimationSnapshotId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}
