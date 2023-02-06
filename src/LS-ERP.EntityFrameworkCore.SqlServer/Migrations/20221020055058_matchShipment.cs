using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class matchShipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MatchingShipment",
                table: "PurchaseOrderLine",
                type: "bit",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Shipment",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileNameServer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FilePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerID = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shipment", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Shipment_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentDetail",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentID = table.Column<long>(type: "bigint", nullable: true),
                    ContractNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerPurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedQuantity = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    AllocQuantity = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    AllocReceivedQuantity = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    MatchedPO = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentDetail", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShipmentDetail_Shipment_ShipmentID",
                        column: x => x.ShipmentID,
                        principalTable: "Shipment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shipment_CustomerID",
                table: "Shipment",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentDetail_ShipmentID",
                table: "ShipmentDetail",
                column: "ShipmentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentDetail");

            migrationBuilder.DropTable(
                name: "Shipment");

            migrationBuilder.DropColumn(
                name: "MatchingShipment",
                table: "PurchaseOrderLine");
        }
    }
}
