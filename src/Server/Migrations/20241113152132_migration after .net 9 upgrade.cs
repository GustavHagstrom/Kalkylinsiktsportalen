using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class migrationafternet9upgrade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataCell_DataCellFormat_CellFormatId",
                table: "DataCell");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetColumn_SheetSection_SheetSectionTemplateId",
                table: "SheetColumn");

            migrationBuilder.DropTable(
                name: "SheetRowFormat");

            migrationBuilder.DropIndex(
                name: "IX_DataCell_CellFormatId",
                table: "DataCell");

            migrationBuilder.DropColumn(
                name: "Align",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "BackgroundColor",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "BorderStyle",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "DecimalCount",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "DoesIncludeTimeOfDay",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "FontSize",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "FormatType",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "HasBorderBottom",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "HasBorderLeft",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "HasBorderRight",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "HasBorderTop",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "HasThoasandsSeparator",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "IsBold",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "IsItalic",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "IsUnderline",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "Justify",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "TextColor",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "RowCount",
                table: "DataSection");

            migrationBuilder.DropColumn(
                name: "CellFormatId",
                table: "DataCell");

            migrationBuilder.RenameColumn(
                name: "SheetSectionTemplateId",
                table: "SheetColumn",
                newName: "SheetSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_SheetColumn_SheetSectionTemplateId",
                table: "SheetColumn",
                newName: "IX_SheetColumn_SheetSectionId");

            migrationBuilder.RenameColumn(
                name: "WidthPercent",
                table: "DataColumn",
                newName: "Width");

            migrationBuilder.RenameColumn(
                name: "Justify",
                table: "DataCellFormat",
                newName: "VerticalAlign");

            migrationBuilder.RenameColumn(
                name: "Align",
                table: "DataCellFormat",
                newName: "HorizontalAlign");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SheetSection",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "DataSection",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DataCellId",
                table: "DataCellFormat",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DataRow",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataSectionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    Height = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataRow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataRow_DataSection_DataSectionId",
                        column: x => x.DataSectionId,
                        principalTable: "DataSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SheetCellFormat",
                columns: table => new
                {
                    SheetSectionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RowType = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnType = table.Column<int>(type: "INTEGER", nullable: false),
                    HorizontalAlign = table.Column<int>(type: "INTEGER", nullable: true),
                    BackgroundColor = table.Column<string>(type: "TEXT", nullable: true),
                    BorderStyle = table.Column<int>(type: "INTEGER", nullable: false),
                    DecimalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DoesIncludeTimeOfDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatType = table.Column<int>(type: "INTEGER", nullable: false),
                    HasBorderBottom = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasThoasandsSeparator = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsItalic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUnderline = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerticalAlign = table.Column<int>(type: "INTEGER", nullable: true),
                    TextColor = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetCellFormat", x => new { x.SheetSectionId, x.ColumnType, x.RowType });
                    table.ForeignKey(
                        name: "FK_SheetCellFormat_SheetSection_SheetSectionId",
                        column: x => x.SheetSectionId,
                        principalTable: "SheetSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataCellFormat_DataCellId",
                table: "DataCellFormat",
                column: "DataCellId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DataRow_DataSectionId",
                table: "DataRow",
                column: "DataSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataCellFormat_DataCell_DataCellId",
                table: "DataCellFormat",
                column: "DataCellId",
                principalTable: "DataCell",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetColumn_SheetSection_SheetSectionId",
                table: "SheetColumn",
                column: "SheetSectionId",
                principalTable: "SheetSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataCellFormat_DataCell_DataCellId",
                table: "DataCellFormat");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetColumn_SheetSection_SheetSectionId",
                table: "SheetColumn");

            migrationBuilder.DropTable(
                name: "DataRow");

            migrationBuilder.DropTable(
                name: "SheetCellFormat");

            migrationBuilder.DropIndex(
                name: "IX_DataCellFormat_DataCellId",
                table: "DataCellFormat");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SheetSection");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "DataSection");

            migrationBuilder.DropColumn(
                name: "DataCellId",
                table: "DataCellFormat");

            migrationBuilder.RenameColumn(
                name: "SheetSectionId",
                table: "SheetColumn",
                newName: "SheetSectionTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_SheetColumn_SheetSectionId",
                table: "SheetColumn",
                newName: "IX_SheetColumn_SheetSectionTemplateId");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "DataColumn",
                newName: "WidthPercent");

            migrationBuilder.RenameColumn(
                name: "VerticalAlign",
                table: "DataCellFormat",
                newName: "Justify");

            migrationBuilder.RenameColumn(
                name: "HorizontalAlign",
                table: "DataCellFormat",
                newName: "Align");

            migrationBuilder.AddColumn<int>(
                name: "Align",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundColor",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BorderStyle",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DecimalCount",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "DoesIncludeTimeOfDay",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FontSize",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "FormatType",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasBorderBottom",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBorderLeft",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBorderRight",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasBorderTop",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasThoasandsSeparator",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBold",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsItalic",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsUnderline",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Justify",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextColor",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RowCount",
                table: "DataSection",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CellFormatId",
                table: "DataCell",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "SheetRowFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    SheetSectionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Align = table.Column<int>(type: "INTEGER", nullable: true),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    BorderStyle = table.Column<int>(type: "INTEGER", nullable: false),
                    DecimalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DoesIncludeTimeOfDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatType = table.Column<int>(type: "INTEGER", nullable: false),
                    HasBorderBottom = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasThoasandsSeparator = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsItalic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUnderline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Justify = table.Column<int>(type: "INTEGER", nullable: true),
                    RowType = table.Column<int>(type: "INTEGER", nullable: false),
                    TextColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetRowFormat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetRowFormat_SheetSection_SheetSectionId",
                        column: x => x.SheetSectionId,
                        principalTable: "SheetSection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DataCell_CellFormatId",
                table: "DataCell",
                column: "CellFormatId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetRowFormat_SheetSectionId",
                table: "SheetRowFormat",
                column: "SheetSectionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataCell_DataCellFormat_CellFormatId",
                table: "DataCell",
                column: "CellFormatId",
                principalTable: "DataCellFormat",
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
    }
}
