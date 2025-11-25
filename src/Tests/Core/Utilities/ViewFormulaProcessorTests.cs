using Core.Models.EstimationEnteties;
using Core.Constants;
using Core.Utilities;

namespace Tests.Core.Utilities;
[TestFixture]
public class ViewFormulaProcessorTests
{
    //Test for basic placeholders
    [TestCase(Placeholders.Estimation.Properties.Name, "Name")]
    [TestCase(Placeholders.Estimation.Properties.Id, "Id")]
    [TestCase(Placeholders.Estimation.Properties.Description, "Description")]
    [TestCase(Placeholders.Estimation.Properties.Customer, "Customer")]
    [TestCase(Placeholders.Estimation.Properties.Place, "Place")]
    [TestCase(Placeholders.Estimation.Properties.HandlingOfficer, "HandlingOfficer")]
    [TestCase(Placeholders.Estimation.Properties.ConfirmationOfficer, "ConfirmationOfficer")]
    [TestCase(Placeholders.Estimation.Properties.TenderTotal, "1\u00A0000\u00A0000")]
    [TestCase($"{Placeholders.Estimation.Properties.Name} - {Placeholders.Estimation.Properties.Description}", "Name - Description")]
    [TestCase($"{Placeholders.Estimation.Properties.Name} - Calc(5* (1+1))", "Name - 10")]
    [TestCase($"{Placeholders.Estimation.Properties.Name} - AccountSingle(7786)", "Name - 7786")]
    [TestCase("Calc(1/0)", "MATH-ERROR")]

    public void Process_EstimationName_ReturnsEstimationName(string placeholder, string expected)
    {
        //Arrange
        var estimation = new Estimation
        {
            Id = "Id",
            BidconId = "Id",
            Name = "Name",
            Description = "Description",
            Customer = "Customer",
            Location = "Place",
            HandlingOfficer = "HandlingOfficer",
            ConfirmationOfficer = "ConfirmationOfficer",
            TenderTotal = 1000000.49999
        };
        //Act
        var actual = ViewFormulaProcessor.Process(placeholder, estimation);
        //Assert
        Assert.That(actual, Is.EqualTo(expected));
    }
}
