using System.Text;

namespace BlazorServer.Shared.Helpers;

public static class CsvExporter
{
    public static string ToCsv<T>(IEnumerable<T> items)
    {
        var csv = new StringBuilder();
        var properties = typeof(T).GetProperties();

        // Header
        csv.AppendLine(string.Join(",", properties.Select(p => EscapeCsvValue(p.Name))));

        // Rows
        foreach (var item in items)
        {
            var values = properties.Select(p =>
            {
                var value = p.GetValue(item)?.ToString() ?? "";
                return EscapeCsvValue(value);
            });
            csv.AppendLine(string.Join(",", values));
        }

        return csv.ToString();
    }

    private static string EscapeCsvValue(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
        {
            return $"\"{value.Replace("\"", "\"\"")}\"";
        }
        return value;
    }
}
