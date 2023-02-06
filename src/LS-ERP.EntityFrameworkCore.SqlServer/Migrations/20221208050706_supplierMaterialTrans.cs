using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class supplierMaterialTrans : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "StorageImportDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "MaterialTransaction",
                type: "nvarchar(max)",
                nullable: true);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "StorageImportDetail");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "MaterialTransaction");
        }
    }
}
