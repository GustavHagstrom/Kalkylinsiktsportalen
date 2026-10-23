using System.Text.RegularExpressions;

namespace BlazorServer.Modules.Reports.Domain.Views;

public static partial class ReportColor
{
    public static string Normalize(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Color is required.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();
        if (!HexColorRegex().IsMatch(normalized))
            throw new ArgumentException("Color must be an uppercase #RRGGBB hex value without alpha.", nameof(value));

        return normalized;
    }

    public static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : Normalize(value);

    [GeneratedRegex("^#[0-9A-F]{6}$")]
    private static partial Regex HexColorRegex();
}
