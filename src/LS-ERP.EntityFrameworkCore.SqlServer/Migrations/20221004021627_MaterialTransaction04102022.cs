using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class MaterialTransaction04102022 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FabricRequestDetailID",
                table: "MaterialTransaction",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RequestQuantity",
                table: "MaterialTransaction",
                type: "DECIMAL(19,9)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FabricRequestDetailID",
                table: "MaterialTransaction");

            migrationBuilder.DropColumn(
                name: "RequestQuantity",
                table: "MaterialTransaction");
        }
    }
}
