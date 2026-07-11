namespace BlazorServer.Modules.Access.Public.Contracts;

public sealed record AccessDirectoryUser(
    string Id,
    string Name,
    string? AvatarBase64);
