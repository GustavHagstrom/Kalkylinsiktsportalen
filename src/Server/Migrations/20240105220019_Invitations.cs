using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class Invitations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationInvitaion_Organizations_OrganizationId",
                table: "OrganizationInvitaion");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationInvitaion",
                table: "OrganizationInvitaion");

            migrationBuilder.RenameTable(
                name: "OrganizationInvitaion",
                newName: "OrganizationInvitaions");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationInvitaion_OrganizationId",
                table: "OrganizationInvitaions",
                newName: "IX_OrganizationInvitaions_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationInvitaions",
                table: "OrganizationInvitaions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationInvitaions_Organizations_OrganizationId",
                table: "OrganizationInvitaions",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrganizationInvitaions_Organizations_OrganizationId",
                table: "OrganizationInvitaions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganizationInvitaions",
                table: "OrganizationInvitaions");

            migrationBuilder.RenameTable(
                name: "OrganizationInvitaions",
                newName: "OrganizationInvitaion");

            migrationBuilder.RenameIndex(
                name: "IX_OrganizationInvitaions_OrganizationId",
                table: "OrganizationInvitaion",
                newName: "IX_OrganizationInvitaion_OrganizationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganizationInvitaion",
                table: "OrganizationInvitaion",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_OrganizationInvitaion_Organizations_OrganizationId",
                table: "OrganizationInvitaion",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
