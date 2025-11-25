namespace Core.Abstractions;
public interface IApplicationLink
{
    string LinkRoute { get; }
    /// <summary>
    /// Displayed in UI. Use localization.
    /// </summary>
    string Text { get; }
    string Icon { get; }
    string? AuthPolicy { get; }
}
