namespace BlazorServer.Modules.Projects.Domain;

public readonly record struct ProjectId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public readonly record struct ProjectEstimationId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public readonly record struct EstimationVersionId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public readonly record struct ProjectVersionId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}

public readonly record struct ViewScopedChangeSetId(Guid Value)
{
    public bool IsEmpty => Value == Guid.Empty;
}
