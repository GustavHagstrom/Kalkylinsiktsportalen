using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class IdForFormatCLass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SheetCellFormat",
                table: "SheetCellFormat");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "SheetCellFormat",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SheetCellFormat",
                table: "SheetCellFormat",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SheetCellFormat_SheetSectionId",
                table: "SheetCellFormat",
                column: "SheetSectionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SheetCellFormat",
                table: "SheetCellFormat");

            migrationBuilder.DropIndex(
                name: "IX_SheetCellFormat_SheetSectionId",
                table: "SheetCellFormat");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SheetCellFormat");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SheetCellFormat",
                table: "SheetCellFormat",
                columns: new[] { "SheetSectionId", "ColumnType", "RowType" });
        }
    }
}
