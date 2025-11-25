using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class EstimationEnteties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SheetItem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstimationId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ParentId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    RowType = table.Column<int>(type: "INTEGER", nullable: false),
                    Quantity = table.Column<double>(type: "REAL", nullable: true),
                    Remark = table.Column<string>(type: "TEXT", nullable: false),
                    Unit = table.Column<string>(type: "TEXT", nullable: true),
                    LayerItemUnitCost = table.Column<double>(type: "REAL", nullable: true),
                    SheetItemId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetItem_SheetItem_ParentId",
                        column: x => x.ParentId,
                        principalTable: "SheetItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SheetItem_SheetItem_SheetItemId",
                        column: x => x.SheetItemId,
                        principalTable: "SheetItem",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Estimation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    TenantId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BidconId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Customer = table.Column<string>(type: "TEXT", nullable: true),
                    Place = table.Column<string>(type: "TEXT", nullable: true),
                    HandlingOfficer = table.Column<string>(type: "TEXT", nullable: true),
                    ConfirmationOfficer = table.Column<string>(type: "TEXT", nullable: true),
                    TenderTotal = table.Column<double>(type: "REAL", nullable: true),
                    NetSheetId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estimation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Estimation_SheetItem_NetSheetId",
                        column: x => x.NetSheetId,
                        principalTable: "SheetItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Estimation_NetSheetId",
                table: "Estimation",
                column: "NetSheetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SheetItem_ParentId",
                table: "SheetItem",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetItem_SheetItemId",
                table: "SheetItem",
                column: "SheetItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Estimation");

            migrationBuilder.DropTable(
                name: "SheetItem");
        }
    }
}
