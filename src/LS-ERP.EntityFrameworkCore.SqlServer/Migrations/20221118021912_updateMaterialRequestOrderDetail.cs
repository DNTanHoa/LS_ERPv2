using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateMaterialRequestOrderDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RequestQuantity",
                table: "MaterialRequestOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SizeSortIndex",
                table: "MaterialRequestOrderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestQuantity",
                table: "MaterialRequestOrderDetails");

            migrationBuilder.DropColumn(
                name: "SizeSortIndex",
                table: "MaterialRequestOrderDetails");
        }
    }
}
