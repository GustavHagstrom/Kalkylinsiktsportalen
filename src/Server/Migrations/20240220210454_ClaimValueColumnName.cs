using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class ClaimValueColumnName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessId",
                table: "UserAccess",
                newName: "AccessClaimValue");

            migrationBuilder.RenameColumn(
                name: "AccessId",
                table: "RoleAccess",
                newName: "AccessClaimValue");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AccessClaimValue",
                table: "UserAccess",
                newName: "AccessId");

            migrationBuilder.RenameColumn(
                name: "AccessClaimValue",
                table: "RoleAccess",
                newName: "AccessId");
        }
    }
}
