namespace BlazorServer.Shared.Abstractions;
public interface IAccessClaimInfo
{
    /// <summary>
    /// Value of claim. Needs to be constant and nevere changed as its stored in DB
    /// </summary>
    string Value { get; }
    /// <summary>
    /// Display name in UI. Use localization here
    /// </summary>
    string Name { get; }
    /// <summary>
    /// Description of the access claim. Use localization here
    /// </summary>
    string Description { get; }
}
