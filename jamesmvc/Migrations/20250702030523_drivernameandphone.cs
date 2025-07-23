using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jamesmvc.Migrations
{
    /// <inheritdoc />
    public partial class drivernameandphone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "DriverProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "DriverProfiles",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "DriverProfiles");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "DriverProfiles");
        }
    }
}
