using System.Globalization;
using BlazorServer.Modules.Reports.Views.Components;
using BlazorServer.Modules.Reports.Public.Contracts.Views;
using BlazorServer.Modules.Reports.Public.Contracts.Views.DataSection;

namespace BlazorServer.Tests;

[TestFixture]
public class DataCellFormulaEvaluatorTests
{
    [Test]
    public void GetDisplayValue_ReturnsRawValue_WhenValueIsNotFormula()
    {
        var cell = CreateCell("Hej världen");

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell);

        Assert.That(result, Is.EqualTo("Hej världen"));
    }

    [Test]
    public void GetDisplayValue_EvaluatesArithmeticFormula()
    {
        var cell = CreateCell("=1+2*3", textType: TextType.Number, decimalCount: 0, hasThousandsSeparator: false);

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell);

        Assert.That(result, Is.EqualTo("7"));
    }

    [Test]
    public void GetDisplayValue_ReturnsEstimationProperty_WhenFormulaContainsSingleReference()
    {
        var cell = CreateCell("=Estimation.Customer");
        var estimation = CreateEstimationProperties(customer: "ACME");

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell, estimation);

        Assert.That(result, Is.EqualTo("ACME"));
    }

    [Test]
    public void GetDisplayValue_EvaluatesEstimationReferenceInsideArithmeticExpression()
    {
        var cell = CreateCell("=Estimation.Reconciled + 5", textType: TextType.Number, decimalCount: 0, hasThousandsSeparator: false);
        var estimation = CreateEstimationProperties(reconciled: true);

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell, estimation);

        Assert.That(result, Is.EqualTo("6"));
    }

    [Test]
    public void GetDisplayValue_ConcatenatesStringLiteralAndEstimationValue()
    {
        var cell = CreateCell("=\"Hej, det kostar \" + Estimation.Customer");
        var estimation = CreateEstimationProperties(customer: "1200 kr");

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell, estimation);

        Assert.That(result, Is.EqualTo("Hej, det kostar 1200 kr"));
    }

    [Test]
    public void GetDisplayValue_FormatsDateTime_UsingTextFormat()
    {
        var cell = CreateCell("=Estimation.CreateDate", textType: TextType.DateTime, dateTimeFormat: "yyyy-MM-dd");
        var estimation = CreateEstimationProperties(createDate: new DateTime(2025, 02, 03, 10, 30, 00));

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell, estimation);

        Assert.That(result, Is.EqualTo("2025-02-03"));
    }

    [Test]
    public void GetDisplayValue_ReturnsRawValue_WhenFormulaIsInvalid()
    {
        var cell = CreateCell("=invalid +");

        var result = DataCellFormulaEvaluator.GetDisplayValue(cell);

        Assert.That(result, Is.EqualTo("=invalid +"));
    }

    [Test]
    public void GetDisplayValue_FormatsNumber_UsingCurrentCulture()
    {
        var previousCulture = CultureInfo.CurrentCulture;
        var previousUiCulture = CultureInfo.CurrentUICulture;

        try
        {
            var culture = CultureInfo.GetCultureInfo("sv-SE");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;

            var cell = CreateCell("=1234.5", textType: TextType.Number, decimalCount: 2, hasThousandsSeparator: true);

            var result = DataCellFormulaEvaluator.GetDisplayValue(cell);

            Assert.That(result, Is.EqualTo("1 234,50"));
        }
        finally
        {
            CultureInfo.CurrentCulture = previousCulture;
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private static DataCellDto CreateCell(
        string rawValue,
        TextType textType = TextType.PlainText,
        int decimalCount = 2,
        bool hasThousandsSeparator = true,
        string dateTimeFormat = "yyyy-MM-dd")
    {
        return new DataCellDto
        {
            RawValue = rawValue,
            TextFormat = new TextFormatDto
            {
                Type = textType,
                DecimalCount = decimalCount,
                HasThoasandsSeparator = hasThousandsSeparator,
                DayTimeFormat = dateTimeFormat
            }
        };
    }

    private static IReadOnlyDictionary<string, object?> CreateEstimationProperties(
        string customer = "",
        bool reconciled = false,
        DateTime? createDate = null)
    {
        return new Dictionary<string, object?>
        {
            ["Customer"] = customer,
            ["Reconciled"] = reconciled,
            ["CreateDate"] = createDate
        };
    }
}
