using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateShippingPlanDetail221129 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetWeight",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossWeight",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
           
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionOrders_Customer_CustomerID",
                table: "ProductionOrders");

            migrationBuilder.DropIndex(
                name: "IX_ProductionOrders_CustomerID",
                table: "ProductionOrders");

            migrationBuilder.AlterColumn<decimal>(
                name: "Volume",
                table: "ShippingPlanDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(19,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "NetWeight",
                table: "ShippingPlanDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(19,9)");

            migrationBuilder.AlterColumn<decimal>(
                name: "GrossWeight",
                table: "ShippingPlanDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(19,9)");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerID",
                table: "ProductionOrders",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldNullable: true);
        }
    }
}
