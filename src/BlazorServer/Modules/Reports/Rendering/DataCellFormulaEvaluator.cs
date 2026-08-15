using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using BlazorServer.Modules.Reports.Domain.Views;
using BlazorServer.Modules.Reports.Domain.Views.DataSection;

namespace BlazorServer.Modules.Reports.Rendering;

public static partial class DataCellFormulaEvaluator
{
    public const string ErrorDisplayValue = "#ERROR!";

    public static string GetDisplayValue(
        DataCell dataCell,
        IReadOnlyDictionary<string, object?>? estimationProperties = null)
    {
        ArgumentNullException.ThrowIfNull(dataCell);

        var evaluatedValue = EvaluateCellExpression(dataCell.CellExpression, estimationProperties);
        return FormatValue(evaluatedValue, dataCell.ValueFormat ?? dataCell.TextFormat?.ToValueFormat() ?? ValueFormat.Text);
    }

    private static object? EvaluateCellExpression(
        string? cellExpression,
        IReadOnlyDictionary<string, object?>? estimationProperties)
    {
        if (string.IsNullOrWhiteSpace(cellExpression))
            return string.Empty;

        if (!IsFormula(cellExpression))
            return cellExpression;

        var expression = cellExpression[1..].Trim();
        if (string.IsNullOrWhiteSpace(expression))
            return string.Empty;

        var resolvedExpression = ReplaceEstimationReferences(expression, estimationProperties, out var singleResolvedValue);
        if (singleResolvedValue is not null)
            return singleResolvedValue;

        if (TryEvaluateExpression(resolvedExpression, out var expressionResult))
            return expressionResult;

        if (TryEvaluateConcatenation(expression, estimationProperties, out var concatenationResult))
            return concatenationResult;

        return ErrorDisplayValue;
    }

    private static bool TryEvaluateExpression(string expression, out object? result)
    {
        try
        {
            using var dataTable = new DataTable();
            result = dataTable.Compute(expression, string.Empty);
            result = result == DBNull.Value ? string.Empty : result;
            return true;
        }
        catch
        {
            result = null;
            return false;
        }
    }

    private static bool IsFormula(string rawValue)
    {
        return rawValue.TrimStart().StartsWith('=');
    }

    private static string ReplaceEstimationReferences(
        string expression,
        IReadOnlyDictionary<string, object?>? estimationProperties,
        out object? singleResolvedValue)
    {
        singleResolvedValue = null;

        if (estimationProperties is null)
            return expression;

        var trimmedExpression = expression.Trim();
        if (IsSingleEstimationReference(trimmedExpression))
        {
            var singlePropertyValue = GetEstimationPropertyValue(trimmedExpression, estimationProperties);
            if (singlePropertyValue is not null)
            {
                singleResolvedValue = singlePropertyValue;
                return expression;
            }
        }

        return EstimationReferenceRegex().Replace(expression, match =>
        {
            var propertyValue = GetEstimationPropertyValue(match.Value, estimationProperties);
            return ConvertToFormulaLiteral(propertyValue);
        });
    }

    private static bool TryEvaluateConcatenation(
        string expression,
        IReadOnlyDictionary<string, object?>? estimationProperties,
        out object? result)
    {
        result = null;

        var segments = SplitTopLevelConcatenation(expression);
        if (segments.Count <= 1)
            return false;

        var parts = new List<string>(segments.Count);
        foreach (var segment in segments)
        {
            if (!TryEvaluateConcatenationSegment(segment, estimationProperties, out var segmentValue))
                return false;

            parts.Add(Convert.ToString(segmentValue, CultureInfo.CurrentCulture) ?? string.Empty);
        }

        result = string.Concat(parts);
        return true;
    }

    private static bool TryEvaluateConcatenationSegment(
        string segment,
        IReadOnlyDictionary<string, object?>? estimationProperties,
        out object? result)
    {
        result = null;

        var trimmedSegment = segment.Trim();
        if (string.IsNullOrWhiteSpace(trimmedSegment))
            return false;

        if (TryParseStringLiteral(trimmedSegment, out var stringLiteral))
        {
            result = stringLiteral;
            return true;
        }

        if (estimationProperties is not null && IsSingleEstimationReference(trimmedSegment))
        {
            result = GetEstimationPropertyValue(trimmedSegment, estimationProperties) ?? string.Empty;
            return true;
        }

        var resolvedSegment = ReplaceEstimationReferences(trimmedSegment, estimationProperties, out var singleResolvedValue);
        if (singleResolvedValue is not null)
        {
            result = singleResolvedValue;
            return true;
        }

        return TryEvaluateExpression(resolvedSegment, out result);
    }

    private static List<string> SplitTopLevelConcatenation(string expression)
    {
        var segments = new List<string>();
        var startIndex = 0;
        var parenthesisDepth = 0;
        var stringDelimiter = '\0';

        for (var index = 0; index < expression.Length; index++)
        {
            var currentCharacter = expression[index];

            if (stringDelimiter != '\0')
            {
                if (currentCharacter == stringDelimiter)
                    stringDelimiter = '\0';

                continue;
            }

            switch (currentCharacter)
            {
                case '\'' or '"':
                    stringDelimiter = currentCharacter;
                    break;
                case '(':
                    parenthesisDepth++;
                    break;
                case ')':
                    parenthesisDepth--;
                    break;
                case '+' when parenthesisDepth == 0:
                    segments.Add(expression[startIndex..index]);
                    startIndex = index + 1;
                    break;
            }
        }

        if (stringDelimiter != '\0' || parenthesisDepth != 0)
            return [expression];

        segments.Add(expression[startIndex..]);
        return segments;
    }

    private static bool TryParseStringLiteral(string value, out string result)
    {
        result = string.Empty;

        if (value.Length < 2)
            return false;

        var delimiter = value[0];
        if ((delimiter != '"' && delimiter != '\'') || value[^1] != delimiter)
            return false;

        result = value[1..^1];
        return true;
    }

    private static object? GetEstimationPropertyValue(
        string reference,
        IReadOnlyDictionary<string, object?> estimationProperties)
    {
        const string prefix = "Estimation.";
        if (!reference.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            return null;

        var propertyName = reference[prefix.Length..];
        return estimationProperties.TryGetValue(propertyName, out var value)
            ? value
            : null;
    }

    private static string ConvertToFormulaLiteral(object? value)
    {
        return value switch
        {
            null => "0",
            byte number => number.ToString(CultureInfo.InvariantCulture),
            sbyte number => number.ToString(CultureInfo.InvariantCulture),
            short number => number.ToString(CultureInfo.InvariantCulture),
            ushort number => number.ToString(CultureInfo.InvariantCulture),
            int number => number.ToString(CultureInfo.InvariantCulture),
            uint number => number.ToString(CultureInfo.InvariantCulture),
            long number => number.ToString(CultureInfo.InvariantCulture),
            ulong number => number.ToString(CultureInfo.InvariantCulture),
            float number => number.ToString(CultureInfo.InvariantCulture),
            double number => number.ToString(CultureInfo.InvariantCulture),
            decimal number => number.ToString(CultureInfo.InvariantCulture),
            bool boolean => boolean ? "1" : "0",
            DateTime dateTime => dateTime.ToOADate().ToString(CultureInfo.InvariantCulture),
            _ => $"'{(value.ToString() ?? string.Empty).Replace("'", "''", StringComparison.Ordinal)}'"
        };
    }

    private static string FormatValue(object? value, ValueFormat valueFormat)
    {
        return valueFormat.Kind switch
        {
            ValueFormatKind.Number => FormatNumber(value, valueFormat),
            ValueFormatKind.Date or ValueFormatKind.DateTimeMinute => FormatDateTime(value, valueFormat),
            _ => Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty
        };
    }

    private static string FormatNumber(object? value, ValueFormat valueFormat)
    {
        if (!TryConvertToDecimal(value, out var decimalValue))
            return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;

        return ReportFormatting.FormatNumber(decimalValue, valueFormat);
    }

    private static string FormatDateTime(object? value, ValueFormat valueFormat)
    {
        if (!TryConvertToDateTime(value, out var dateTimeValue))
            return Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;

        var format = valueFormat.Kind == ValueFormatKind.DateTimeMinute
            ? "yyyy-MM-dd HH:mm"
            : "yyyy-MM-dd";

        return dateTimeValue.ToString(format, CultureInfo.CurrentCulture);
    }

    private static bool TryConvertToDecimal(object? value, out decimal decimalValue)
    {
        switch (value)
        {
            case null:
                decimalValue = default;
                return false;
            case decimal directDecimal:
                decimalValue = directDecimal;
                return true;
            case byte or sbyte or short or ushort or int or uint or long or ulong or float or double:
                decimalValue = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                return true;
            case string stringValue when decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.CurrentCulture, out decimalValue):
                return true;
            case string stringValue when decimal.TryParse(stringValue, NumberStyles.Any, CultureInfo.InvariantCulture, out decimalValue):
                return true;
            default:
                decimalValue = default;
                return false;
        }
    }

    private static bool TryConvertToDateTime(object? value, out DateTime dateTimeValue)
    {
        switch (value)
        {
            case DateTime directDateTime:
                dateTimeValue = directDateTime;
                return true;
            case string stringValue when DateTime.TryParse(stringValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out dateTimeValue):
                return true;
            case string stringValue when DateTime.TryParse(stringValue, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeValue):
                return true;
            default:
                dateTimeValue = default;
                return false;
        }
    }

    [GeneratedRegex(@"Estimation\.[A-Za-z_][A-Za-z0-9_]*")]
    private static partial Regex EstimationReferenceRegex();

    [GeneratedRegex(@"^Estimation\.[A-Za-z_][A-Za-z0-9_]*$", RegexOptions.IgnoreCase)]
    private static partial Regex SingleEstimationReferenceRegex();

    private static bool IsSingleEstimationReference(string expression)
    {
        return SingleEstimationReferenceRegex().IsMatch(expression);
    }
}
