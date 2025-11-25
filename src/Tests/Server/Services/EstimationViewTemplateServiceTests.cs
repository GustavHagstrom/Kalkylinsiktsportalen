//using Core.Data;
//using Shared.Services.EstimationView;
//using Shared.DTOs.EstimationView;
//using Mapster;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Moq;

//namespace Tests.Server.Services;
//[TestFixture]
//public class EstimationViewTemplateServiceTests
//{
//    private TestDbContext _dbContext;
//    private IEstimationViewTemplateService _service;
//    private readonly string _organizationId = "orgId";

//    [SetUp]
//    public void Setup()
//    {
//        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
//            .UseInMemoryDatabase(databaseName: "TestDb" + TestContext.CurrentContext.Test.Name)
//            .Options;

//        var loggerMock = new Mock<ILogger<IEstimationViewTemplateService>>();

//        _dbContext = new TestDbContext(options);
//        _service = new EstimationViewTemplateService(_dbContext, new EstimationViewTemplateUpdater(),  loggerMock.Object);
//    }
//    [Test]
//    public async Task UpsertAsync_ShouldInsertNewRecordWithNestedValues()
//    {
//        var entity = EstimationViewTemplateDtoSamples.Sample();

//        await _service.UpsertAsync(entity, _organizationId);
//        var actual = await _dbContext.EstimationViewTemplates
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns)
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns).ThenInclude(x => x.CellFormat)
//            .Include(x => x.HeaderOrFooters)
//            .Include(x => x.DataSections).ThenInclude(x => x.Columns)
//            .Include(x => x.DataSections).ThenInclude(x => x.Cells).ThenInclude(x => x.Format)
//            .FirstOrDefaultAsync();

//        Assert.That(actual, Is.Not.Null);
//        Assert.That(actual.HeaderOrFooters.Count, Is.EqualTo(1));
//        Assert.That(actual.SheetSections.Count, Is.EqualTo(2));
//        Assert.That(actual.SheetSections.First().Columns.Count, Is.EqualTo(1));
//        Assert.That(actual.SheetSections.First().Columns.First().CellFormat, Is.Not.Null);
//        Assert.That(actual.DataSections.Count, Is.EqualTo(2));
//        Assert.That(actual.DataSections.First().Cells.Count, Is.EqualTo(1));
//        Assert.That(actual.DataSections.First().Columns.Count, Is.EqualTo(1));
//        Assert.That(actual.DataSections.First().Cells.First().Format, Is.Not.Null);
//        Assert.That(actual.DataSections.First().Columns.FirstOrDefault(), Is.Not.Null);
//    }
//    [Test]
//    public async Task UpsertAsync_ShouldUpdateIncludingNested()
//    {
//        var entity = EstimationViewTemplateDtoSamples.Sample();
//        await _service.UpsertAsync(entity, _organizationId);
//        var dbEntety = await _dbContext.EstimationViewTemplates
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns)
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns).ThenInclude(x => x.CellFormat)
//            .Include(x => x.HeaderOrFooters)
//            .Include(x => x.DataSections).ThenInclude(x => x.Columns)
//            .Include(x => x.DataSections).ThenInclude(x => x.Cells).ThenInclude(x => x.Format)
//            .FirstOrDefaultAsync();
//        var dto = dbEntety!.Adapt<EstimationViewTemplateDto>();
//        dto.DataSections.First().Cells.First().Format.FontFamily = "Arial";

//        await _service.UpsertAsync(dto, _organizationId);
//        var actual = await _dbContext.EstimationViewTemplates
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns)
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns).ThenInclude(x => x.CellFormat)
//            .Include(x => x.HeaderOrFooters)
//            .Include(x => x.DataSections).ThenInclude(x => x.Columns)
//            .Include(x => x.DataSections).ThenInclude(x => x.Cells).ThenInclude(x => x.Format)
//            .FirstOrDefaultAsync();

        
//        Assert.That(actual?.DataSections.First().Cells.First().Format.FontFamily, Is.EqualTo("Arial"));
//    }
//    [Test]
//    public async Task UpsertAsync_ShouldUpdateDeletedNested()
//    {
//        var entity = EstimationViewTemplateDtoSamples.Sample();
//        await _service.UpsertAsync(entity, _organizationId);
//        var dbEntety = await _dbContext.EstimationViewTemplates
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns)
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns).ThenInclude(x => x.CellFormat)
//            .Include(x => x.HeaderOrFooters)
//            .Include(x => x.DataSections).ThenInclude(x => x.Columns)
//            .Include(x => x.DataSections).ThenInclude(x => x.Cells).ThenInclude(x => x.Format)
//            .FirstOrDefaultAsync();

//        var dto = dbEntety!.Adapt<EstimationViewTemplateDto>();
//        dto.DataSections.Remove(dto.DataSections.First());
//        dto.SheetSections.Remove(dto.SheetSections.First());
//        await _service.UpsertAsync(dto, _organizationId);

//        var actual = await _dbContext.EstimationViewTemplates
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns)
//            .Include(x => x.SheetSections).ThenInclude(x => x.Columns).ThenInclude(x => x.CellFormat)
//            .Include(x => x.HeaderOrFooters)
//            .Include(x => x.DataSections).ThenInclude(x => x.Columns)
//            .Include(x => x.DataSections).ThenInclude(x => x.Cells).ThenInclude(x => x.Format)
//            .FirstOrDefaultAsync();


//        Assert.That(actual?.DataSections.Count(), Is.EqualTo(1));
//        Assert.That(actual?.SheetSections.Count(), Is.EqualTo(1));
//    }
//}
