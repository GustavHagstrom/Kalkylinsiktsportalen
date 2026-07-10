using BlazorServer.Modules.Bidcon.Domain;
using BlazorServer.Modules.Bidcon.Public.Contracts;

namespace BlazorServer.Tests.Modules.Bidcon;

[TestFixture]
public class EstimationSnapshotTests
{
    [Test]
    public void Create_PreservesVersionedPayloadAndMetadata()
    {
        var snapshotId = new EstimationSnapshotId(Guid.Parse("44444444-4444-4444-4444-444444444444"));
        var organizationId = new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a");
        var bidconEstimationId = new BidconEstimationId("EST-100");
        var sourceEditDate = new DateTime(2026, 06, 29, 08, 30, 00, DateTimeKind.Utc);
        var importedAt = new DateTimeOffset(2026, 06, 30, 10, 15, 00, TimeSpan.Zero);
        var payload = CreatePayload();

        var snapshot = EstimationSnapshot.Create(
            snapshotId,
            organizationId,
            bidconEstimationId,
            sourceEditDate,
            importedAt,
            payload);

        Assert.Multiple(() =>
        {
            Assert.That(snapshot.Id, Is.EqualTo(snapshotId));
            Assert.That(snapshot.OrganizationId, Is.EqualTo(organizationId));
            Assert.That(snapshot.BidconEstimationId, Is.EqualTo(bidconEstimationId));
            Assert.That(snapshot.SchemaVersion, Is.EqualTo(SnapshotSchemaVersion.V1));
            Assert.That(snapshot.SourceEditDate, Is.EqualTo(sourceEditDate));
            Assert.That(snapshot.ImportedAt, Is.EqualTo(importedAt));
            Assert.That(snapshot.Payload, Is.SameAs(payload));
            Assert.That(snapshot.Payload.Name, Is.EqualTo("Kvarteret Linden"));
            Assert.That(snapshot.Payload.BidPhaseResources[0].Remark, Is.EqualTo("{replace:mark}"));
            Assert.That(snapshot.Payload.BidPhaseStructures!.Estimation!.NetSheet!.Children[0].Reference, Is.EqualTo("NET-1"));
        });
    }

    [Test]
    public void Create_ThrowsDomainException_WhenSnapshotIdIsEmpty()
    {
        var exception = Assert.Throws<BidconDomainException>(() =>
            EstimationSnapshot.Create(
                new EstimationSnapshotId(Guid.Empty),
                new OrganizationId("1c928774-3e22-4a94-9146-8784fb54d67a"),
                new BidconEstimationId("EST-100"),
                new DateTime(2026, 06, 29, 08, 30, 00, DateTimeKind.Utc),
                DateTimeOffset.UtcNow,
                CreatePayload()));

        Assert.That(exception!.Code, Is.EqualTo("estimationSnapshot.id.required"));
    }

    private static EstimationSnapshotPayloadV1 CreatePayload()
    {
        var resource = new ResourceSnapshotPayload(
            AccountCode: "4000",
            AccountDecription: "Material",
            ResourceTypeCode: "MAT",
            ResourceTypeDescription: "Material",
            SheetType: SheetType.NetSheet,
            DSCode: "DS1",
            DSCodeDescription: "Schakt",
            EstimationCode: "01.001",
            Description: "Schakt",
            LocationCode: "L1",
            LocationDescription: "Etapp 1",
            TradeCode: "T1",
            TradeCodeDescription: "Mark",
            Cost: 1000m,
            CompletedCost: 0m,
            Quantity: 2.5m,
            CompletedQuantity: 0m,
            Unit: "m3",
            ArticleNumber: "A-1",
            ExternalId: "EXT-1",
            CO2Value: 10m,
            Weight: 20m,
            UnitNetPrice: 400m,
            UnitCustomerPrice: 450m,
            Waste: 0.05m,
            BSAB96: "B",
            BSAB96Description: "BSAB",
            BD96: "BD",
            Remark: "{replace:mark}",
            BD96Description: "BD text",
            TimeListCode: "TL",
            TimeListCodeDescription: "Time list",
            WorkResultDSCode: "WR",
            WorkResultDescription: "Work result",
            WorkResultQuantity: 2.5m,
            WorkResultUnit: "m3",
            DesignElementBSAB96Code: "DE",
            DesignElementDescription: "Design",
            DesignElementQuantity: 1m,
            DesignElementUnit: "st",
            MixedElementDescription: "Mixed",
            MixedElementQuantity: 1m,
            MixedElementUnit: "st",
            HierarchyLevel1Description: "Mark",
            HierarchyLevel2Description: "Schakt",
            HierarchyLevel3Description: "");

        return new EstimationSnapshotPayloadV1(
            Name: "Kvarteret Linden",
            Description: "Anbud",
            Discipline: DisciplineType.Bygg,
            CurrentWorkSheet: WorksheetStatusType.Bid,
            CurrentWorkSheetSubState: WorksheetSubStatusType.Bid,
            Customer: "Bestallare",
            HandlingOfficer: "Anna",
            Id: Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Location: "Stockholm",
            CreateDate: new DateTime(2026, 06, 01),
            EditDate: new DateTime(2026, 06, 29),
            InitiationDate: new DateTime(2026, 06, 02),
            Reconciled: false,
            Reconciliation: false,
            EstimationUri: "bidcon://estimation/EST-100",
            ProjectId: Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Link: "https://bidcon.example/EST-100",
            BidPhaseResources: [resource],
            ProductionPhaseResources: [],
            Variations: [new VariationDetailsSnapshotPayload(false, true, 123.45, "V1", "Tillagg", "3000", [resource])],
            BidPhaseStructures: new StructureSnapshotPayload(
                Accounts: null,
                ResourceTypes: null,
                DSCodes: null,
                Locations: null,
                Estimation: new StructureEstimationSnapshotPayload(
                    None: null,
                    NetSheet: new StructureEstimationNodeSnapshotPayload(
                        "Netto",
                        [new StructureEstimationNodeSnapshotPayload("Schakt", [], "01.001", "{replace:mark}", RowType.Posting, 1, true, 2.5m, "m3", "NET-1")],
                        "NET",
                        "",
                        RowType.Group,
                        0,
                        true,
                        1m,
                        "st",
                        ""),
                    TenderSheet: null,
                    PrelimsSheet: null,
                    SummarySheet: null,
                    LockedStagesSheet: null,
                    ReconciliationSheet: null),
                TradeCodes: null),
            VariationProductionPhaseStructures: null,
            DynamicProperties: [new DynamicPropertySnapshotPayload("dp1", "Dynamic", "Value")],
            TenderRateExVAT: 100,
            TenderRateIncVAT: 125,
            IsCompleted: false);
    }
}
