namespace BlazorServer.Modules.Access.Domain;

public sealed record FunctionalAccessGrantTarget
{
    public FunctionalAccessGrantTarget(FunctionalAccessGrantTargetType type, string id)
    {
        if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Target id cannot be null or empty.", nameof(id));

        Type = type;
        Id = id;
    }

    public FunctionalAccessGrantTargetType Type { get; }
    public string Id { get; }

    public static FunctionalAccessGrantTarget User(string userId) =>
        new(FunctionalAccessGrantTargetType.User, userId);

    public static FunctionalAccessGrantTarget Role(string roleId) =>
        new(FunctionalAccessGrantTargetType.Role, roleId);
}
