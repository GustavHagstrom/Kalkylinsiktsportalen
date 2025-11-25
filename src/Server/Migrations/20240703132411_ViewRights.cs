using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class ViewRights : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSection_Views_EstimationViewId",
                table: "DataSection");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetSection_Views_ViewTemplateId",
                table: "SheetSection");

            migrationBuilder.DropTable(
                name: "Footer");

            migrationBuilder.DropTable(
                name: "Header");

            migrationBuilder.RenameColumn(
                name: "ViewTemplateId",
                table: "SheetSection",
                newName: "ViewId");

            migrationBuilder.RenameIndex(
                name: "IX_SheetSection_ViewTemplateId",
                table: "SheetSection",
                newName: "IX_SheetSection_ViewId");

            migrationBuilder.RenameColumn(
                name: "EstimationViewId",
                table: "DataSection",
                newName: "ViewId");

            migrationBuilder.RenameIndex(
                name: "IX_DataSection_EstimationViewId",
                table: "DataSection",
                newName: "IX_DataSection_ViewId");

            migrationBuilder.AddColumn<bool>(
                name: "IsFooter",
                table: "DataSection",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHeader",
                table: "DataSection",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "RoleViews",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ViewId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleViews", x => new { x.RoleId, x.ViewId });
                    table.ForeignKey(
                        name: "FK_RoleViews_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleViews_Views_ViewId",
                        column: x => x.ViewId,
                        principalTable: "Views",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserViews",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    ViewId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViews", x => new { x.UserId, x.ViewId });
                    table.ForeignKey(
                        name: "FK_UserViews_Views_ViewId",
                        column: x => x.ViewId,
                        principalTable: "Views",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RoleViews_ViewId",
                table: "RoleViews",
                column: "ViewId");

            migrationBuilder.CreateIndex(
                name: "IX_UserViews_ViewId",
                table: "UserViews",
                column: "ViewId");

            migrationBuilder.AddForeignKey(
                name: "FK_DataSection_Views_ViewId",
                table: "DataSection",
                column: "ViewId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SheetSection_Views_ViewId",
                table: "SheetSection",
                column: "ViewId",
                principalTable: "Views",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DataSection_Views_ViewId",
                table: "DataSection");

            migrationBuilder.DropForeignKey(
                name: "FK_SheetSection_Views_ViewId",
                table: "SheetSection");

            migrationBuilder.DropTable(
                name: "RoleViews");

            migrationBuilder.DropTable(
                name: "UserViews");

            migrationBuilder.DropColumn(
                name: "IsFooter",
                table: "DataSection");

            migrationBuilder.DropColumn(
                name: "IsHeader",
                table: "DataSection");

            migrationBuilder.RenameColumn(
                name: "ViewId",
                table: "SheetSection",
                newName: "ViewTemplateId");

            migrationBuilder.RenameIndex(
                name: "IX_SheetSection_ViewId",
                table: "SheetSection",
                newName: "IX_SheetSection_ViewTemplateId");

            migrationBuilder.RenameColumn(
                name: "ViewId",
                table: "DataSection",
                newName: "EstimationViewId");

            migrationBuilder.RenameIndex(
                name: "IX_DataSection_ViewId",
                table: "DataSection",
                newName: "IX_DataSection_EstimationViewId");

            migrationBuilder.CreateTable(
                name: "Footer",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Formula = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Footer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Footer_Views_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "Views",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Header",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    EstimationViewTemplateId = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Formula = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Header", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Header_Views_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "Views",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.AddForeignKey(
                name: "FK_DataSection_Views_EstimationViewId",
                table: "DataSection",
                column: "EstimationViewId",
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
    }
}
