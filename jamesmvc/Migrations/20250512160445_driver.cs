using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jamesmvc.Migrations
{
    /// <inheritdoc />
    public partial class driver : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedDriverId",
                table: "LogisticsViewModel",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DriverProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ServiceCity = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceDistrict = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DriverProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DriverProfiles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LogisticsViewModel_AssignedDriverId",
                table: "LogisticsViewModel",
                column: "AssignedDriverId");

            migrationBuilder.CreateIndex(
                name: "IX_DriverProfiles_UserId",
                table: "DriverProfiles",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LogisticsViewModel_AspNetUsers_AssignedDriverId",
                table: "LogisticsViewModel",
                column: "AssignedDriverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LogisticsViewModel_AspNetUsers_AssignedDriverId",
                table: "LogisticsViewModel");

            migrationBuilder.DropTable(
                name: "DriverProfiles");

            migrationBuilder.DropIndex(
                name: "IX_LogisticsViewModel_AssignedDriverId",
                table: "LogisticsViewModel");

            migrationBuilder.DropColumn(
                name: "AssignedDriverId",
                table: "LogisticsViewModel");
        }
    }
}
