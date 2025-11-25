using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class formatRevmap : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataCell_CellFormat_CellFormatId",
                table: "DataCell");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetColumn_CellFormat_CellFormatId",
                table: "SheetColumn");

            migrationBuilder.DropTable(
                name: "CellFormat");

            migrationBuilder.DropIndex(
                name: "IX_SheetColumn_CellFormatId",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "CellFormatId",
                table: "SheetColumn");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "SheetItem",
                newName: "RowType");

            migrationBuilder.RenameColumn(
                name: "WidthPercent",
                table: "SheetColumn",
                newName: "Width");

            migrationBuilder.AddColumn<string>(
                name: "FontFamily",
                table: "Views",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Align",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<bool>(
                name: "IsVisible",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Justify",
                table: "SheetColumn",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "TextColor",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataCellFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FontFamily = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TextColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    IsBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsItalic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUnderline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Align = table.Column<int>(type: "INTEGER", nullable: false),
                    Justify = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatType = table.Column<int>(type: "INTEGER", nullable: false),
                    HasThoasandsSeparator = table.Column<bool>(type: "INTEGER", nullable: false),
                    DecimalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DoesIncludeTimeOfDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderBottom = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderStyle = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataCellFormat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SheetRowFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    RowType = table.Column<int>(type: "INTEGER", nullable: false),
                    SheetSectionId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    BackgroundColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    TextColor = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    IsBold = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsItalic = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsUnderline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Align = table.Column<int>(type: "INTEGER", nullable: false),
                    Justify = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatType = table.Column<int>(type: "INTEGER", nullable: false),
                    HasThoasandsSeparator = table.Column<bool>(type: "INTEGER", nullable: false),
                    DecimalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DoesIncludeTimeOfDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    HasBorderBottom = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderStyle = table.Column<int>(type: "INTEGER", nullable: false)
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataCell_DataCellFormat_CellFormatId",
                table: "DataCell");

            migrationBuilder.DropTable(
                name: "DataCellFormat");

            migrationBuilder.DropTable(
                name: "SheetRowFormat");

            migrationBuilder.DropColumn(
                name: "FontFamily",
                table: "Views");

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
                name: "IsVisible",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "Justify",
                table: "SheetColumn");

            migrationBuilder.DropColumn(
                name: "TextColor",
                table: "SheetColumn");

            migrationBuilder.RenameColumn(
                name: "RowType",
                table: "SheetItem",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "Width",
                table: "SheetColumn",
                newName: "WidthPercent");

            migrationBuilder.AddColumn<string>(
                name: "CellFormatId",
                table: "SheetColumn",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "CellFormat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Align = table.Column<int>(type: "INTEGER", nullable: false),
                    Bold = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderBottom = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderStyle = table.Column<int>(type: "INTEGER", nullable: false),
                    BorderTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    DecimalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    FontFamily = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatType = table.Column<int>(type: "INTEGER", nullable: false),
                    IncludeTimeOfDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    Italic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Justify = table.Column<int>(type: "INTEGER", nullable: false),
                    ThoasandsSeparator = table.Column<bool>(type: "INTEGER", nullable: false),
                    Underline = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellFormat", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SheetColumn_CellFormatId",
                table: "SheetColumn",
                column: "CellFormatId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataCell_CellFormat_CellFormatId",
                table: "DataCell",
                column: "CellFormatId",
                principalTable: "CellFormat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetColumn_CellFormat_CellFormatId",
                table: "SheetColumn",
                column: "CellFormatId",
                principalTable: "CellFormat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
