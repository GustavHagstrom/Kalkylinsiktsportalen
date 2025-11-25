using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    public partial class initial : Migration
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsDarkMode = table.Column<bool>(type: "INTEGER", nullable: false),
                    ActiveOrganizationId = table.Column<Guid>(type: "TEXT", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "TEXT", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: true),
                    SecurityStamp = table.Column<string>(type: "TEXT", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumber = table.Column<string>(type: "TEXT", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "TEXT", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Organizations_ActiveOrganizationId",
                        column: x => x.ActiveOrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BidconAccessCredentials",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Server = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Database = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    User = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    ServerAuthentication = table.Column<bool>(type: "INTEGER", nullable: false),
                    UseDesktopBidconLink = table.Column<bool>(type: "INTEGER", nullable: false),
                    DesktopPort = table.Column<int>(type: "INTEGER", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BidconAccessCredentials", x => x.OrganizationId);
                    table.ForeignKey(
                        name: "FK_BidconAccessCredentials_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EstimationViewTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EstimationViewTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EstimationViewTemplates_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserLimit = table.Column<int>(type: "INTEGER", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Licenses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    ClaimType = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimValue = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderKey = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "TEXT", nullable: true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    RoleId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    LoginProvider = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganizations",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    OrganizationId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganizations", x => new { x.UserId, x.OrganizationId });
                    table.ForeignKey(
                        name: "FK_UserOrganizations_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOrganizations_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSectionTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    RowCount = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimationViewTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Position = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimationViewTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    SheetType = table.Column<int>(type: "INTEGER", nullable: false),
                    EstimationViewTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
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
                name: "UserViewTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 450, nullable: false),
                    EstimationViewTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserViewTemplate_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserViewTemplate_EstimationViewTemplates_EstimationViewTemplateId",
                        column: x => x.EstimationViewTemplateId,
                        principalTable: "EstimationViewTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CellTemplate",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Row = table.Column<int>(type: "INTEGER", nullable: false),
                    Column = table.Column<int>(type: "INTEGER", nullable: false),
                    Value = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    DataSectionTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "DataColumn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    WidthPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    DataSectionTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataColumn_DataSectionTemplate_DataSectionTemplateId",
                        column: x => x.DataSectionTemplateId,
                        principalTable: "DataSectionTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SheetColumn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    WidthPercent = table.Column<int>(type: "INTEGER", nullable: false),
                    ColumnType = table.Column<int>(type: "INTEGER", nullable: false),
                    NetSheetSectionTemplateId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SheetColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SheetColumn_SheetSectionTemplate_NetSheetSectionTemplateId",
                        column: x => x.NetSheetSectionTemplateId,
                        principalTable: "SheetSectionTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CellFormat",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    FontFamily = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    FontSize = table.Column<int>(type: "INTEGER", nullable: false),
                    Bold = table.Column<bool>(type: "INTEGER", nullable: false),
                    Italic = table.Column<bool>(type: "INTEGER", nullable: false),
                    Underline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Align = table.Column<int>(type: "INTEGER", nullable: false),
                    Justify = table.Column<int>(type: "INTEGER", nullable: false),
                    FormatType = table.Column<int>(type: "INTEGER", nullable: false),
                    ThoasandsSeparator = table.Column<bool>(type: "INTEGER", nullable: false),
                    DecimalCount = table.Column<int>(type: "INTEGER", nullable: false),
                    IncludeTimeOfDay = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderLeft = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderTop = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderRight = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderBottom = table.Column<bool>(type: "INTEGER", nullable: false),
                    BorderStyle = table.Column<int>(type: "INTEGER", nullable: false),
                    SheetColumnId = table.Column<Guid>(type: "TEXT", nullable: true),
                    CellTemplateId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CellFormat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CellFormat_CellTemplate_CellTemplateId",
                        column: x => x.CellTemplateId,
                        principalTable: "CellTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CellFormat_SheetColumn_SheetColumnId",
                        column: x => x.SheetColumnId,
                        principalTable: "SheetColumn",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ActiveOrganizationId",
                table: "AspNetUsers",
                column: "ActiveOrganizationId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

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
                name: "IX_DataColumn_DataSectionTemplateId",
                table: "DataColumn",
                column: "DataSectionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSectionTemplate_EstimationViewTemplateId",
                table: "DataSectionTemplate",
                column: "EstimationViewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EstimationViewTemplates_OrganizationId",
                table: "EstimationViewTemplates",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_HeaderOrFooter_EstimationViewTemplateId",
                table: "HeaderOrFooter",
                column: "EstimationViewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Licenses_OrganizationId",
                table: "Licenses",
                column: "OrganizationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SheetColumn_NetSheetSectionTemplateId",
                table: "SheetColumn",
                column: "NetSheetSectionTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_SheetSectionTemplate_EstimationViewTemplateId",
                table: "SheetSectionTemplate",
                column: "EstimationViewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganizations_OrganizationId",
                table: "UserOrganizations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserViewTemplate_EstimationViewTemplateId",
                table: "UserViewTemplate",
                column: "EstimationViewTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_UserViewTemplate_UserId",
                table: "UserViewTemplate",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "BidconAccessCredentials");

            migrationBuilder.DropTable(
                name: "CellFormat");

            migrationBuilder.DropTable(
                name: "DataColumn");

            migrationBuilder.DropTable(
                name: "HeaderOrFooter");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "UserOrganizations");

            migrationBuilder.DropTable(
                name: "UserViewTemplate");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "CellTemplate");

            migrationBuilder.DropTable(
                name: "SheetColumn");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "DataSectionTemplate");

            migrationBuilder.DropTable(
                name: "SheetSectionTemplate");

            migrationBuilder.DropTable(
                name: "EstimationViewTemplates");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
