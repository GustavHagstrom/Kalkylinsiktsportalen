using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class Stuff2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataColumn_DataSection_DataSectionTemplateId",
                table: "DataColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_DataSection_EstimationViewTemplates_EstimationViewId",
                table: "DataSection");

            migrationBuilder.DropForeignKey(
                name: "FK_Footer_EstimationViewTemplates_EstimationViewTemplateId",
                table: "Footer");

            migrationBuilder.DropForeignKey(
                name: "FK_Header_EstimationViewTemplates_EstimationViewTemplateId",
                table: "Header");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetSection_EstimationViewTemplates_ViewTemplateId",
                table: "SheetSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EstimationViewTemplates",
                table: "EstimationViewTemplates");

            migrationBuilder.DropColumn(
                name: "EstimationViewTemplateId",
                table: "DataSection");

            migrationBuilder.RenameTable(
                name: "EstimationViewTemplates",
                newName: "Views");

            migrationBuilder.RenameColumn(
                name: "DataSectionTemplateId",
                table: "DataColumn",
                newName: "DataSectionId");

            migrationBuilder.RenameIndex(
                name: "IX_DataColumn_DataSectionTemplateId",
                table: "DataColumn",
                newName: "IX_DataColumn_DataSectionId");

            migrationBuilder.AlterColumn<string>(
                name: "EstimationViewId",
                table: "DataSection",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Views",
                table: "Views",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DataColumn_DataSection_DataSectionId",
                table: "DataColumn",
                column: "DataSectionId",
                principalTable: "DataSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DataSection_Views_EstimationViewId",
                table: "DataSection",
                column: "EstimationViewId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Footer_Views_EstimationViewTemplateId",
                table: "Footer",
                column: "EstimationViewTemplateId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Header_Views_EstimationViewTemplateId",
                table: "Header",
                column: "EstimationViewTemplateId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetSection_Views_ViewTemplateId",
                table: "SheetSection",
                column: "ViewTemplateId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataColumn_DataSection_DataSectionId",
                table: "DataColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_DataSection_Views_EstimationViewId",
                table: "DataSection");

            migrationBuilder.DropForeignKey(
                name: "FK_Footer_Views_EstimationViewTemplateId",
                table: "Footer");

            migrationBuilder.DropForeignKey(
                name: "FK_Header_Views_EstimationViewTemplateId",
                table: "Header");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetSection_Views_ViewTemplateId",
                table: "SheetSection");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Views",
                table: "Views");

            migrationBuilder.RenameTable(
                name: "Views",
                newName: "EstimationViewTemplates");

            migrationBuilder.RenameColumn(
                name: "DataSectionId",
                table: "DataColumn",
                newName: "DataSectionTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_DataColumn_DataSectionId",
                table: "DataColumn",
                newName: "IX_DataColumn_DataSectionTemplateId");

            migrationBuilder.AlterColumn<string>(
                name: "EstimationViewId",
                table: "DataSection",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "EstimationViewTemplateId",
                table: "DataSection",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EstimationViewTemplates",
                table: "EstimationViewTemplates",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DataColumn_DataSection_DataSectionTemplateId",
                table: "DataColumn",
                column: "DataSectionTemplateId",
                principalTable: "DataSection",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DataSection_EstimationViewTemplates_EstimationViewId",
                table: "DataSection",
                column: "EstimationViewId",
                principalTable: "EstimationViewTemplates",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Footer_EstimationViewTemplates_EstimationViewTemplateId",
                table: "Footer",
                column: "EstimationViewTemplateId",
                principalTable: "EstimationViewTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Header_EstimationViewTemplates_EstimationViewTemplateId",
                table: "Header",
                column: "EstimationViewTemplateId",
                principalTable: "EstimationViewTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetSection_EstimationViewTemplates_ViewTemplateId",
                table: "SheetSection",
                column: "ViewTemplateId",
                principalTable: "EstimationViewTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
