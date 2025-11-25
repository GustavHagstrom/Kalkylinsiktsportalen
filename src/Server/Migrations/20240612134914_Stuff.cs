using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class Stuff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CellFormat_CellTemplate_CellTemplateId",
                table: "CellFormat");

            migrationBuilder.DropForeignKey(
                name: "FK_CellFormat_SheetColumn_SheetColumnId",
                table: "CellFormat");

            migrationBuilder.DropForeignKey(
                name: "FK_DataColumn_DataSectionTemplate_DataSectionTemplateId",
                table: "DataColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetColumn_SheetSectionTemplate_NetSheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.DropTable(
                name: "CellTemplate");

            migrationBuilder.DropTable(
                name: "HeaderOrFooter");

            migrationBuilder.DropTable(
                name: "SheetSectionTemplate");

            migrationBuilder.DropTable(
                name: "DataSectionTemplate");

            migrationBuilder.DropIndex(
                name: "IX_SheetColumn_NetSheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.DropIndex(
                name: "IX_CellFormat_CellTemplateId",
                table: "CellFormat");

            migrationBuilder.DropIndex(
                name: "IX_CellFormat_SheetColumnId",
                table: "CellFormat");

            migrationBuilder.DropColumn(
                name: "NetSheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "CellTemplateId",
                table: "CellFormat");

            migrationBuilder.DropColumn(
                name: "SheetColumnId",
                table: "CellFormat");

            migrationBuilder.RenameColumn(
                name: "RowType",
                table: "SheetItem",
                newName: "Type");

            migrationBuilder.AddColumn<double>(
                name: "LayerItemUnitAskingPrice",
                table: "SheetItem",
                type: "REAL",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CellFormatId",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SheetSectionTemplateId",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DataSection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    RowCount = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstimationViewId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSection_EstimationViewTemplates_EstimationViewId",
                        column: x => x.EstimationViewId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Footer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Footer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Footer_EstimationViewTemplates_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Header",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Header_EstimationViewTemplates_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SheetSection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    SheetType = table.Column<int>(type: "INTEGER", nullable: false),
                    ViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetSection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetSection_EstimationViewTemplates_ViewTemplateId",
                        column: x => x.ViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataCell",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Row = table.Column<int>(type: "INTEGER", nullable: false),
                    Column = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataSectionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CellFormatId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataCell", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataCell_CellFormat_CellFormatId",
                        column: x => x.CellFormatId,
                        principalTable: "CellFormat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataCell_DataSection_DataSectionId",
                        column: x => x.DataSectionId,
                        principalTable: "DataSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SheetColumn_CellFormatId",
                table: "SheetColumn",
                column: "CellFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetColumn_SheetSectionTemplateId",
                table: "SheetColumn",
                column: "SheetSectionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DataCell_CellFormatId",
                table: "DataCell",
                column: "CellFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_DataCell_DataSectionId",
                table: "DataCell",
                column: "DataSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSection_EstimationViewId",
                table: "DataSection",
                column: "EstimationViewId");

            migrationBuilder.CreateIndex(
                name: "IX_Footer_EstimationViewTemplateId",
                table: "Footer",
                column: "EstimationViewTemplateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Header_EstimationViewTemplateId",
                table: "Header",
                column: "EstimationViewTemplateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SheetSection_ViewTemplateId",
                table: "SheetSection",
                column: "ViewTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataColumn_DataSection_DataSectionTemplateId",
                table: "DataColumn",
                column: "DataSectionTemplateId",
                principalTable: "DataSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetColumn_CellFormat_CellFormatId",
                table: "SheetColumn",
                column: "CellFormatId",
                principalTable: "CellFormat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetColumn_SheetSection_SheetSectionTemplateId",
                table: "SheetColumn",
                column: "SheetSectionTemplateId",
                principalTable: "SheetSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataColumn_DataSection_DataSectionTemplateId",
                table: "DataColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetColumn_CellFormat_CellFormatId",
                table: "SheetColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetColumn_SheetSection_SheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.DropTable(
                name: "DataCell");

            migrationBuilder.DropTable(
                name: "Footer");

            migrationBuilder.DropTable(
                name: "Header");

            migrationBuilder.DropTable(
                name: "SheetSection");

            migrationBuilder.DropTable(
                name: "DataSection");

            migrationBuilder.DropIndex(
                name: "IX_SheetColumn_CellFormatId",
                table: "SheetColumn");

            migrationBuilder.DropIndex(
                name: "IX_SheetColumn_SheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "LayerItemUnitAskingPrice",
                table: "SheetItem");

            migrationBuilder.DropColumn(
                name: "CellFormatId",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "SheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "SheetItem",
                newName: "RowType");

            migrationBuilder.AddColumn<string>(
                name: "NetSheetSectionTemplateId",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 450,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CellTemplateId",
                table: "CellFormat",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SheetColumnId",
                table: "CellFormat",
                type: "TEXT",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataSectionTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    RowCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSectionTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSectionTemplate_EstimationViewTemplates_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeaderOrFooter",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeaderOrFooter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeaderOrFooter_EstimationViewTemplates_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SheetSectionTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    SheetType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetSectionTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetSectionTemplate_EstimationViewTemplates_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CellTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    DataSectionTemplateId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    Column = table.Column<int>(type: "INTEGER", nullable: false),
                    Row = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CellTemplate_DataSectionTemplate_DataSectionTemplateId",
                        column: x => x.DataSectionTemplateId,
                        principalTable: "DataSectionTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SheetColumn_NetSheetSectionTemplateId",
                table: "SheetColumn",
                column: "NetSheetSectionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CellFormat_CellTemplateId",
                table: "CellFormat",
                column: "CellTemplateId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CellFormat_SheetColumnId",
                table: "CellFormat",
                column: "SheetColumnId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CellTemplate_DataSectionTemplateId",
                table: "CellTemplate",
                column: "DataSectionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSectionTemplate_EstimationViewTemplateId",
                table: "DataSectionTemplate",
                column: "EstimationViewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderOrFooter_EstimationViewTemplateId",
                table: "HeaderOrFooter",
                column: "EstimationViewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetSectionTemplate_EstimationViewTemplateId",
                table: "SheetSectionTemplate",
                column: "EstimationViewTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CellFormat_CellTemplate_CellTemplateId",
                table: "CellFormat",
                column: "CellTemplateId",
                principalTable: "CellTemplate",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CellFormat_SheetColumn_SheetColumnId",
                table: "CellFormat",
                column: "SheetColumnId",
                principalTable: "SheetColumn",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DataColumn_DataSectionTemplate_DataSectionTemplateId",
                table: "DataColumn",
                column: "DataSectionTemplateId",
                principalTable: "DataSectionTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetColumn_SheetSectionTemplate_NetSheetSectionTemplateId",
                table: "SheetColumn",
                column: "NetSheetSectionTemplateId",
                principalTable: "SheetSectionTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
