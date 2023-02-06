using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addOrderNoPOLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderNo",
                table: "PurchaseOrderLine",
                type: "nvarchar(max)",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderNo",
                table: "PurchaseOrderLine");

        }
    }
}
