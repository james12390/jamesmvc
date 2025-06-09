using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace jamesmvc.Migrations
{
    /// <inheritdoc />
    public partial class name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackingRecords_LogisticsViewModel_LogisticsViewModel",
                table: "TrackingRecords");

            migrationBuilder.DropTable(
                name: "LogisticsViewModel");

            migrationBuilder.DropIndex(
                name: "IX_TrackingRecords_LogisticsViewModel",
                table: "TrackingRecords");

            migrationBuilder.DropColumn(
                name: "LogisticsViewModel",
                table: "TrackingRecords");

            migrationBuilder.CreateTable(
                name: "LogisticsOrder",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AssignedDriverId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticsOrder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticsOrder_AspNetUsers_AssignedDriverId",
                        column: x => x.AssignedDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingRecords_LogisticsOrderId",
                table: "TrackingRecords",
                column: "LogisticsOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticsOrder_AssignedDriverId",
                table: "LogisticsOrder",
                column: "AssignedDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingRecords_LogisticsOrder_LogisticsOrderId",
                table: "TrackingRecords",
                column: "LogisticsOrderId",
                principalTable: "LogisticsOrder",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrackingRecords_LogisticsOrder_LogisticsOrderId",
                table: "TrackingRecords");

            migrationBuilder.DropTable(
                name: "LogisticsOrder");

            migrationBuilder.DropIndex(
                name: "IX_TrackingRecords_LogisticsOrderId",
                table: "TrackingRecords");

            migrationBuilder.AddColumn<int>(
                name: "LogisticsViewModel",
                table: "TrackingRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LogisticsViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssignedDriverId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DeliveryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ItemDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceiverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderDistrict = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LogisticsViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LogisticsViewModel_AspNetUsers_AssignedDriverId",
                        column: x => x.AssignedDriverId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingRecords_LogisticsViewModel",
                table: "TrackingRecords",
                column: "LogisticsViewModel");

            migrationBuilder.CreateIndex(
                name: "IX_LogisticsViewModel_AssignedDriverId",
                table: "LogisticsViewModel",
                column: "AssignedDriverId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrackingRecords_LogisticsViewModel_LogisticsViewModel",
                table: "TrackingRecords",
                column: "LogisticsViewModel",
                principalTable: "LogisticsViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
