using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jamesmvc.Migrations
{
    /// <inheritdoc />
    public partial class customermessagerd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "CustomerMessages",
                newName: "SenderId");

            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "CustomerMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "CustomerMessages");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "CustomerMessages",
                newName: "CustomerId");
        }
    }
}
