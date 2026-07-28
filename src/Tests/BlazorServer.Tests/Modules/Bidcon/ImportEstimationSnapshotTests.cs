using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Features;
using BlazorServer.Modules.Bidcon.Features.ImportEstimationSnapshot;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.DTOs;
using BlazorServer.Modules.Bidcon.Infrastructure.BidconApi.Mapping;
using BlazorServer.Modules.Bidcon.Infrastructure.Persistence;
using BlazorServer.Modules.Bidcon.Public.Contracts;
using BlazorServer.Identity;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class ImportEstimationSnapshotTests
{
    [Test]
    public async Task ImportEstimationSnapshot_SavesCompleteSnapshot_WhenBidconReturnsDetails()
    {
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var bidconEstimationId = new BidconEstimationId("EST-100");
        var store = new RecordingEstimationSnapshotStore();
        var handler = new ImportEstimationSnapshotCommandHandler(
            new StubBidconEstimationSnapshotSource(CreateSnapshotDetails()),
            store);

        var beforeImport = DateTimeOffset.UtcNow;
        var result = await handler.HandleAsync(
            new ImportEstimationSnapshotCommand(organizationId, bidconEstimationId),
            CancellationToken.None);
        var afterImport = DateTimeOffset.UtcNow;

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<EstimationSnapshotImported>());
            var imported = (EstimationSnapshotImported)result;
            Assert.That(imported.WasImported, Is.True);
            Assert.That(imported.SnapshotId.Value, Is.Not.EqualTo(Guid.Empty));
            Assert.That(imported.BidconEstimationId, Is.EqualTo(bidconEstimationId));
            Assert.That(imported.SourceEditDate, Is.EqualTo(new DateTime(2026, 06, 29, 08, 30, 00, DateTimeKind.Utc)));
            Assert.That(imported.ImportedAt, Is.InRange(beforeImport, afterImport));

            Assert.That(store.SavedSnapshot, Is.Not.Null);
            Assert.That(store.SavedSnapshot!.Id, Is.EqualTo(imported.SnapshotId));
            Assert.That(store.SavedSnapshot.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(store.SavedSnapshot.BidconEstimationId, Is.EqualTo(bidconEstimationId));
            Assert.That(store.SavedSnapshot.SourceEditDate, Is.EqualTo(new DateTime(2026, 06, 29, 08, 30, 00, DateTimeKind.Utc)));
            Assert.That(store.SavedSnapshot.ImportedAt, Is.EqualTo(imported.ImportedAt));
            Assert.That(store.SavedSnapshot.Payload.Name, Is.EqualTo("Kvarteret Linden"));
            Assert.That(store.SavedSnapshot.Payload.Discipline, Is.EqualTo(DisciplineType.Bygg));
            Assert.That(store.SavedSnapshot.Payload.BidPhaseResources[0].Remark, Is.EqualTo("{replace:mark}"));
            Assert.That(store.SavedSnapshot.Payload.Variations[0].Resources[0].ExternalId, Is.EqualTo("EXT-1"));
            Assert.That(store.SavedSnapshot.Payload.BidPhaseStructures!.Estimation!.NetSheet!.Children[0].Reference, Is.EqualTo("NET-1"));
            Assert.That(store.SavedSnapshot.Payload.DynamicProperties[0].Value, Is.EqualTo("Value"));
        });
    }

    [Test]
    public async Task ImportEstimationSnapshot_ReturnsNotFoundError_WhenBidconReturnsNoDetails()
    {
        var store = new RecordingEstimationSnapshotStore();
        var handler = new ImportEstimationSnapshotCommandHandler(
            new StubBidconEstimationSnapshotSource(null),
            store);

        var result = await handler.HandleAsync(
            new ImportEstimationSnapshotCommand(
                new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a"),
                new BidconEstimationId("EST-100")),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ImportEstimationSnapshotNotFound>());
            var notFound = (ImportEstimationSnapshotNotFound)result;
            Assert.That(notFound.ErrorCode, Is.EqualTo("bidcon.estimation.notFound"));
            Assert.That(notFound.ErrorField, Is.EqualTo(nameof(ImportEstimationSnapshotCommand.BidconEstimationId)));
            Assert.That(store.SavedSnapshot, Is.Null);
        });
    }

    [Test]
    public async Task ImportEstimationSnapshot_ReturnsValidationErrors_WhenCommandIsMissingRequiredIds()
    {
        var source = new RecordingBidconEstimationSnapshotSource();
        var store = new RecordingEstimationSnapshotStore();
        var handler = new ImportEstimationSnapshotCommandHandler(source, store);

        var result = await handler.HandleAsync(
            new ImportEstimationSnapshotCommand(
                new OrganizationId(string.Empty),
                new BidconEstimationId(" ")),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<InvalidImportEstimationSnapshotRequest>());
            var invalidRequest = (InvalidImportEstimationSnapshotRequest)result;
            Assert.That(invalidRequest.ValidationErrors.Select(error => error.Code), Is.EquivalentTo(
            [
                "bidcon.import.organization.required",
                "bidcon.import.estimation.required"
            ]));
            Assert.That(source.WasCalled, Is.False);
            Assert.That(store.SavedSnapshot, Is.Null);
        });
    }

    [Test]
    public async Task ImportEstimationSnapshot_ReturnsImportError_WhenBidconClientFails()
    {
        var store = new RecordingEstimationSnapshotStore();
        var handler = new ImportEstimationSnapshotCommandHandler(
            new FailingBidconEstimationSnapshotSource(
                new BidconFeatureSourceException(
                    "bidcon.credentials.missing",
                    nameof(ImportEstimationSnapshotCommand.OrganizationId),
                    "Bidcon credentials are missing.")),
            store);

        var result = await handler.HandleAsync(
            new ImportEstimationSnapshotCommand(
                new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a"),
                new BidconEstimationId("EST-100")),
            CancellationToken.None);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.TypeOf<ImportEstimationSnapshotBidconUnavailable>());
            var unavailable = (ImportEstimationSnapshotBidconUnavailable)result;
            Assert.That(unavailable.ErrorCode, Is.EqualTo("bidcon.credentials.missing"));
            Assert.That(unavailable.ErrorField, Is.EqualTo(nameof(ImportEstimationSnapshotCommand.OrganizationId)));
            Assert.That(store.SavedSnapshot, Is.Null);
        });
    }

    private static BidconEstimationSnapshotSourceDetails CreateSnapshotDetails()
    {
        var estimationDetails = CreateEstimationDetails();
        return new BidconEstimationSnapshotSourceDetails(
            estimationDetails.EditDate,
            estimationDetails.ToSnapshotPayload());
    }

    [Test]
    public void BidconExternalEnums_MatchSnapshotEnums()
    {
        Assert.Multiple(() =>
        {
            AssertEnumValuesMatch<DisciplineTypeDto, DisciplineType>();
            AssertEnumValuesMatch<WorksheetStatusTypeDto, WorksheetStatusType>();
            AssertEnumValuesMatch<WorksheetSubStatusTypeDto, WorksheetSubStatusType>();
            AssertEnumValuesMatch<SheetTypeDto, SheetType>();
            AssertEnumValuesMatch<RowTypeDto, RowType>();
        });
    }

    private static EstimationDetailsDto CreateEstimationDetails()
    {
        var resource = new ResourceDto
        {
            AccountCode = "4000",
            AccountDecription = "Material",
            ResourceTypeCode = "MAT",
            ResourceTypeDescription = "Material",
            SheetType = SheetTypeDto.NetSheet,
            DSCode = "DS1",
            DSCodeDescription = "Schakt",
            EstimationCode = "01.001",
            Description = "Schakt",
            LocationCode = "L1",
            LocationDescription = "Etapp 1",
            TradeCode = "T1",
            TradeCodeDescription = "Mark",
            Cost = 1000m,
            Quantity = 2.5m,
            Unit = "m3",
            ArticleNumber = "A-1",
            ExternalId = "EXT-1",
            CO2Value = 10m,
            Weight = 20m,
            UnitNetPrice = 400m,
            UnitCustomerPrice = 450m,
            Waste = 0.05m,
            BSAB96 = "B",
            BSAB96Description = "BSAB",
            BD96 = "BD",
            Remark = "{replace:mark}",
            BD96Description = "BD text",
            TimeListCode = "TL",
            TimeListCodeDescription = "Time list",
            WorkResultDSCode = "WR",
            WorkResultDescription = "Work result",
            WorkResultQuantity = 2.5m,
            WorkResultUnit = "m3",
            DesignElementBSAB96Code = "DE",
            DesignElementDescription = "Design",
            DesignElementQuantity = 1m,
            DesignElementUnit = "st",
            MixedElementDescription = "Mixed",
            MixedElementQuantity = 1m,
            MixedElementUnit = "st",
            HierarchyLevel1Description = "Mark",
            HierarchyLevel2Description = "Schakt"
        };

        return new EstimationDetailsDto
        {
            Name = "Kvarteret Linden",
            Description = "Anbud",
            Discipline = DisciplineTypeDto.Bygg,
            CurrentWorkSheet = WorksheetStatusTypeDto.Bid,
            CurrentWorkSheetSubState = WorksheetSubStatusTypeDto.Bid,
            Customer = "Bestallare",
            HandlingOfficer = "Anna",
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Location = "Stockholm",
            CreateDate = new DateTime(2026, 06, 01),
            EditDate = new DateTime(2026, 06, 29, 08, 30, 00, DateTimeKind.Utc),
            InitiationDate = new DateTime(2026, 06, 02),
            EstimationUri = "bidcon://estimation/EST-100",
            ProjectId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Link = "https://bidcon.example/EST-100",
            BidPhaseResources = [resource],
            Variations =
            [
                new VariationDetailsDto
                {
                    AffectsRevenueBudget = true,
                    ApprovedAmount = 123.45,
                    Code = "V1",
                    Description = "Tillagg",
                    RevenueBudgetAccount = "3000",
                    Resources = [resource]
                }
            ],
            BidPhaseStructures = new Structure
            {
                Estimation = new StructureEstimationDto
                {
                    NetSheet = new StructureEstimationNodeDto
                    {
                        Description = "Netto",
                        Code = "NET",
                        RowType = RowTypeDto.Group,
                        IsActive = true,
                        Quantity = 1m,
                        Unit = "st",
                        Children =
                        [
                            new StructureEstimationNodeDto
                            {
                                Description = "Schakt",
                                Code = "01.001",
                                Remark = "{replace:mark}",
                                RowType = RowTypeDto.Posting,
                                RowOrder = 1,
                                IsActive = true,
                                Quantity = 2.5m,
                                Unit = "m3",
                                Reference = "NET-1"
                            }
                        ]
                    }
                }
            },
            DynamicProperties =
            [
                new DynamicPropertiesDto
                {
                    Identifier = "dp1",
                    Description = "Dynamic",
                    Value = "Value"
                }
            ],
            TenderRateExVAT = 100,
            TenderRateIncVAT = 125
        };
    }

    private static void AssertEnumValuesMatch<TExternal, TSnapshot>()
        where TExternal : struct, Enum
        where TSnapshot : struct, Enum
    {
        var externalValues = Enum.GetValues<TExternal>()
            .Select(value => (Name: value.ToString(), Value: Convert.ToInt32(value)))
            .OrderBy(value => value.Value)
            .ToArray();
        var snapshotValues = Enum.GetValues<TSnapshot>()
            .Select(value => (Name: value.ToString(), Value: Convert.ToInt32(value)))
            .OrderBy(value => value.Value)
            .ToArray();

        Assert.That(snapshotValues, Is.EqualTo(externalValues));
    }

    private sealed class StubBidconEstimationSnapshotSource(BidconEstimationSnapshotSourceDetails? estimationDetails) : IBidconEstimationSnapshotSource
    {
        public Task<BidconEstimationSnapshotSourceDetails?> GetSnapshotDetailsAsync(
            BidconEstimationId bidconEstimationId,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(estimationDetails);
        }
    }

    private sealed class RecordingEstimationSnapshotStore : IEstimationSnapshotStore
    {
        public EstimationSnapshot? SavedSnapshot { get; private set; }

        public Task AddAsync(EstimationSnapshot snapshot, CancellationToken cancellationToken)
        {
            SavedSnapshot = snapshot;
            return Task.CompletedTask;
        }

        public Task<EstimationSnapshot?> GetAsync(EstimationSnapshotId snapshotId, CancellationToken cancellationToken)
        {
            return Task.FromResult(SavedSnapshot?.Id == snapshotId ? SavedSnapshot : null);
        }
    }

    private sealed class RecordingBidconEstimationSnapshotSource : IBidconEstimationSnapshotSource
    {
        public bool WasCalled { get; private set; }

        public Task<BidconEstimationSnapshotSourceDetails?> GetSnapshotDetailsAsync(
            BidconEstimationId bidconEstimationId,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            WasCalled = true;
            return Task.FromResult<BidconEstimationSnapshotSourceDetails?>(null);
        }
    }

    private sealed class FailingBidconEstimationSnapshotSource(Exception exception) : IBidconEstimationSnapshotSource
    {
        private readonly Exception _exception = exception;

        public Task<BidconEstimationSnapshotSourceDetails?> GetSnapshotDetailsAsync(
            BidconEstimationId bidconEstimationId,
            OrganizationId organizationId,
            CancellationToken cancellationToken)
        {
            throw _exception;
        }
    }
}
