using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateShppingPlanDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "ShippingPlanDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "ShippingPlanDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SheetName",
                table: "ShippingPlanDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Color",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "SheetName",
                table: "ShippingPlanDetails");
            
        }
    }
}
