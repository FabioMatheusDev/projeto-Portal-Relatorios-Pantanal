using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortalRelatorios.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Sectors",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR(36)", nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR(36)", nullable: false),
                    Username = table.Column<string>(type: "NVARCHAR(256)", maxLength: 256, nullable: false),
                    Name = table.Column<string>(type: "NVARCHAR(512)", maxLength: 512, nullable: false),
                    Email = table.Column<string>(type: "NVARCHAR(512)", maxLength: 512, nullable: true),
                    IsAdmin = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "NVARCHAR(36)", nullable: false),
                    UserId = table.Column<string>(type: "NVARCHAR(36)", nullable: false),
                    SectorId = table.Column<string>(type: "NVARCHAR(36)", nullable: false),
                    CanView = table.Column<bool>(type: "BOOLEAN", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Permissions_Sectors_SectorId",
                        column: x => x.SectorId,
                        principalTable: "Sectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Permissions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_SectorId",
                table: "Permissions",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_UserId_SectorId",
                table: "Permissions",
                columns: new[] { "UserId", "SectorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Sectors");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
