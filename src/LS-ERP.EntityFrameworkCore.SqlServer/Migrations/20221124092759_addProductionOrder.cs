using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addProductionOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductionOrderLines",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Number = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductionOrderID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LSStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GarmentColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GarmentColorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GarmentSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Season = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerOrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerOderType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BeginProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndProductionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstimateSupplierHandOver = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShippedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ShipmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipVia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PerCartonBox = table.Column<int>(type: "int", nullable: false),
                    Packing = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrderLines", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ProductionOrders",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProductionOrderTypeID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReferenceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimateStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EstimateEndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionOrders", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductionOrderLines");

            migrationBuilder.DropTable(
                name: "ProductionOrders");
        }
    }
}
