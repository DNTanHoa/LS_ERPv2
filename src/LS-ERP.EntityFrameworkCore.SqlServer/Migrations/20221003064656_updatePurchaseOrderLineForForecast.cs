using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updatePurchaseOrderLineForForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ConsumedForecastQuantity",
                table: "PurchaseOrderLine",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConsumedForecastQuantity",
                table: "PurchaseOrderLine");
        }
    }
}
