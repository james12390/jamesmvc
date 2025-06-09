using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jamesmvc.Migrations
{
    /// <inheritdoc />
    public partial class TOPMenuUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LogisticsViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    SenderAddress = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(200)", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticsViewModel", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LogisticsViewModel");
        }
    }
}
