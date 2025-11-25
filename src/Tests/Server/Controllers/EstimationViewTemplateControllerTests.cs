//using Shared.Services.EstimationView;
//using Shared.DTOs.EstimationView;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Moq;
//using System.Security.Claims;
//using static SharedLibrary.Constants.BackendApiEndpoints;

//namespace Tests.Server.Controllers;
//[TestFixture]
//public class EstimationViewTemplateControllerTests
//{
//    private Mock<IEstimationViewTemplateService> _estimationViewTemplateServiceMock;
//    private EstimationViewTemplateController _controller;
//    private readonly string _organizationId = "orgId";

//    [SetUp]
//    public void Setup()
//    {
//        _estimationViewTemplateServiceMock = new Mock<IEstimationViewTemplateService>();
//        _loggerMock = new Mock<ILogger<EstimationViewTemplateController>>();

//        var httpContext = new DefaultHttpContext();
//        httpContext.Request.Headers["device-id"] = "20317";
//        var identity = new ClaimsIdentity();
//        identity.AddClaim(new Claim(ClaimConstants.TenantId, _organizationId));
//        httpContext.User = new ClaimsPrincipal(identity);

//        _controller = new EstimationViewTemplateController(_estimationViewTemplateServiceMock.Object, _loggerMock.Object);
//        _controller.ControllerContext.HttpContext = httpContext;
//    }

//    [Test]
//    public async Task GetAllAsShallowList_ReturnsOk()
//    {
//        // Arrange - Set up mocks and data
//        _estimationViewTemplateServiceMock.Setup(s => s.GetAllAsShallowListAsync(It.IsAny<string>()))
//            .Returns(Task.FromResult<IEnumerable<EstimationViewTemplateDto>?>(EstimationViewTemplateDtoSamples.ListSample));

//        // Act - Call the controller action
//        var result = await _controller.GetAllAsShallowList();

//        // Assert - Verify the result
//        Assert.That(result, Is.TypeOf<OkObjectResult>());
//        var okResult = result as OkObjectResult;
//        Assert.That(okResult?.Value, Is.Not.Null); // Ensure content is not null
//    }

//    [Test]
//    public async Task GetAllAsShallowList_ReturnsNoContent_WhenNoData()
//    {
//        // Arrange - Set up mocks for a scenario with no data
//        _estimationViewTemplateServiceMock.Setup(s => s.GetAllAsShallowListAsync(It.IsAny<string>()))
//            .Returns(Task.FromResult<IEnumerable<EstimationViewTemplateDto>?>(null));

//        // Act - Call the controller action
//        var result = await _controller.GetAllAsShallowList();

//        // Assert - Verify the result
//        Assert.That(result, Is.TypeOf<NoContentResult>());
//    }

//    [Test]
//    public async Task Get_ReturnsOk_WhenRecordExists()
//    {
//        // Arrange - Set up mocks and data
//        Guid validId = Guid.NewGuid();
//        _estimationViewTemplateServiceMock.Setup(s => s.GetSingleDeepAsync(validId, It.IsAny<string>()))
//            .ReturnsAsync(EstimationViewTemplateDtoSamples.Sample());

//        // Act - Call the controller action
//        var result = await _controller.Get(validId);

//        // Assert - Verify the result
//        Assert.That(result, Is.TypeOf<OkObjectResult>());
//        var okResult = result as OkObjectResult;
//        Assert.That(okResult?.Value, Is.Not.Null); // Ensure content is not null
//    }

//    [Test]
//    public async Task Get_ReturnsNoContent_WhenRecordNotFound()
//    {
//        // Arrange - Set up mocks for a scenario where the record is not found
//        _estimationViewTemplateServiceMock.Setup(s => s.GetSingleDeepAsync(It.IsAny<Guid>(), It.IsAny<string>()))
//            .Returns(Task.FromResult<EstimationViewTemplateDto?>(null));

//        // Act - Call the controller action
//        var result = await _controller.Get(Guid.NewGuid()); // Use a non-existing ID

//        // Assert - Verify the result
//        Assert.That(result, Is.TypeOf<NoContentResult>());
//    }

//    [Test]
//    public async Task Upsert_ReturnsOk_WhenSuccessful()
//    {
//        // Arrange - Set up mocks and data
//        var dto = EstimationViewTemplateDtoSamples.Sample();
//        _estimationViewTemplateServiceMock.Setup(s => s.UpsertAsync(dto, It.IsAny<string>())); // Mocking a successful upsert

//        // Act - Call the controller action
//        var result = await _controller.Upsert(dto);

//        // Assert - Verify the result
//        Assert.That(result, Is.TypeOf<OkResult>());
//    }

//    [Test]
//    public async Task Upsert_ReturnsProblem_WhenExceptionThrown()
//    {
//        // Arrange - Set up mocks for a scenario where an exception is thrown during upsert
//        var dto = EstimationViewTemplateDtoSamples.Sample();
//        _estimationViewTemplateServiceMock.Setup(s => s.UpsertAsync(dto, It.IsAny<string>()))
//            .ThrowsAsync(new Exception("Mock exception"));

//        // Act - Call the controller action
//        var result = await _controller.Upsert(dto) as ObjectResult;

//        // Assert - Verify the result
//        Assert.That(result, Is.Not.Null);
//        Assert.That(result.StatusCode, Is.EqualTo(500));
//    }

//    [Test]
//    public async Task Delete_ReturnsOk_WhenSuccessful()
//    {
//        // Arrange - Set up mocks and data
//        Guid validId = Guid.NewGuid();
//        _estimationViewTemplateServiceMock.Setup(s => s.DeleteAsync(validId, It.IsAny<string>())); // Mocking a successful delete

//        // Act - Call the controller action
//        var result = await _controller.Delete(validId);

//        // Assert - Verify the result
//        Assert.That(result, Is.TypeOf<OkResult>());
//    }

//    [Test]
//    public void Delete_ThrowsException_WhenErrorOccurs()
//    {
//        // Arrange - Set up mocks for a scenario where an exception is thrown during delete
//        Guid validId = Guid.NewGuid();
//        _estimationViewTemplateServiceMock.Setup(s => s.DeleteAsync(validId, It.IsAny<string>()))
//            .ThrowsAsync(new Exception("Mock exception"));

//        // Act & Assert - Exception should be thrown directly
//        Assert.ThrowsAsync<Exception>(() => _controller.Delete(validId));
//    }
//}
