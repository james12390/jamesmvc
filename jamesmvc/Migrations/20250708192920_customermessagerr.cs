using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jamesmvc.Migrations
{
    /// <inheritdoc />
    public partial class customermessagerr : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "CustomerMessages",
                newName: "IsFromCustomer");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsFromCustomer",
                table: "CustomerMessages",
                newName: "IsRead");
        }
    }
}
