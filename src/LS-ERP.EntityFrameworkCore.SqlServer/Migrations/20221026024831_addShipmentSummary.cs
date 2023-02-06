using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addShipmentSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShipmentSummary",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShipmentID = table.Column<long>(type: "bigint", nullable: true),
                    VendorID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CustomerPurchaseOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrxDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    MRQ = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeliveryNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaterialCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentSummary", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ShipmentSummary_Shipment_ShipmentID",
                        column: x => x.ShipmentID,
                        principalTable: "Shipment",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShipmentSummary_Vendor_VendorID",
                        column: x => x.VendorID,
                        principalTable: "Vendor",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentSummary_ShipmentID",
                table: "ShipmentSummary",
                column: "ShipmentID");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentSummary_VendorID",
                table: "ShipmentSummary",
                column: "VendorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShipmentSummary");
        }
    }
}
