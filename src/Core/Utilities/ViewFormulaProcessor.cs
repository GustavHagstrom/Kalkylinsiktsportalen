using Core.Models.EstimationEnteties;
using Core.Constants;
using System.Text.RegularExpressions;
using NCalc;

namespace Core.Utilities;
public static class ViewFormulaProcessor
{
    private static readonly Dictionary<string, Func<Estimation, string?>> _formulas = new()
    {
        { Placeholders.Estimation.Properties.Id, estimation => estimation.Id },
        { Placeholders.Estimation.Properties.Name, estimation => estimation.Name },
        { Placeholders.Estimation.Properties.Description, estimation => estimation.Description },
        { Placeholders.Estimation.Properties.Customer, estimation => estimation.Customer },
        { Placeholders.Estimation.Properties.Place, estimation => estimation.Location },
        { Placeholders.Estimation.Properties.HandlingOfficer, estimation => estimation.HandlingOfficer },
        { Placeholders.Estimation.Properties.ConfirmationOfficer, estimation => estimation.ConfirmationOfficer },
        { Placeholders.Estimation.Properties.TenderTotal, estimation => estimation.TenderTotal?.ToString("N0") }
    };
    /// <summary>
    /// Processes the formula and replaces the placeholders with the values from the estimation.
    /// </summary>
    /// <param name="formula"></param>
    /// <param name="estimation"></param>
    /// <returns></returns>
    public static string Process(string formula, Estimation estimation)
    {
        //regex patterns for placeholders
        var placeholderPattern = @"\{([^\}]+)\}";
        var calcFuncPattern = "Calc\\((.*?)\\)";
        var accountSingleFuncPattern = "AccountSingle\\((.*?)\\)";

        var result = Regex.Replace(formula, placeholderPattern, match =>
        {
            var placeholder = match.Groups[0].Value;
            if (_formulas.ContainsKey(placeholder))
            {
                return _formulas[placeholder](estimation) ?? string.Empty;
            }
             return placeholder;
            
        });
        result = Regex.Replace(result, accountSingleFuncPattern, match =>
        {
            var accountValue = match.Groups[1].Value;
            
            return $"{accountValue}";
        });

        result = Regex.Replace(result, calcFuncPattern, match =>
        {
            var formula = match.Groups[1].Value;
            var expression = new Expression(formula);
            var mathResult = expression.Evaluate();
            return mathResult?.ToString() ?? "MATH-ERROR";
        });



        return result;
    }

}
