using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class CleanupLegacyAuthTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account");

            migrationBuilder.DropTable(
                name: "EmployeeRole");

            migrationBuilder.DropTable(
                name: "Role");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    Username = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    MemberID = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__536C85E5260F1375", x => x.Username);
                    table.ForeignKey(
                        name: "FK__Account__MemberI__3C69FB99",
                        column: x => x.MemberID,
                        principalTable: "Member",
                        principalColumn: "MemberID");
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Role__8AFACE3ACEE4216E", x => x.RoleID);
                });

            migrationBuilder.CreateTable(
                name: "EmployeeRole",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__C27FE312BCE697B6", x => new { x.EmployeeID, x.RoleID });
                    table.ForeignKey(
                        name: "FK__EmployeeR__Emplo__46E78A0C",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID");
                    table.ForeignKey(
                        name: "FK__EmployeeR__RoleI__47DBAE45",
                        column: x => x.RoleID,
                        principalTable: "Role",
                        principalColumn: "RoleID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_MemberID",
                table: "Account",
                column: "MemberID");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeRole_RoleID",
                table: "EmployeeRole",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "UQ__Role__8A2B6160D9FD0471",
                table: "Role",
                column: "RoleName",
                unique: true);
        }
    }
}
