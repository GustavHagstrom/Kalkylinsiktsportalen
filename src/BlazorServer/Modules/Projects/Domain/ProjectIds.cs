namespace BlazorServer.Modules.Projects.Domain;

public readonly record struct ProjectId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public readonly record struct BidconEstimationId(string Value)
{
    public bool IsEmpty => string.IsNullOrWhiteSpace(Value);
}

public readonly record struct EstimationSnapshotId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public readonly record struct ProjectVersionId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}
