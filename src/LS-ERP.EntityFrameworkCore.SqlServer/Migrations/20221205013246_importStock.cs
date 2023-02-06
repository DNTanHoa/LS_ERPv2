using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class importStock : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OutputOrder",
                table: "StorageImportDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionMethodCode",
                table: "StorageImportDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "RollNo",
                table: "StorageImportDetail",
                type: "decimal(19,9)",
                precision: 19,
                scale: 9,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<long>(
                name: "StorageDetailID",
                table: "StorageImportDetail",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "Output",
                table: "StorageImport",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FabricPurchaseOrderNumber",
                table: "IssuedLine",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OutputOrder",
                table: "StorageImportDetail");

            migrationBuilder.DropColumn(
                name: "ProductionMethodCode",
                table: "StorageImportDetail");

            migrationBuilder.DropColumn(
                name: "RollNo",
                table: "StorageImportDetail");

            migrationBuilder.DropColumn(
                name: "StorageDetailID",
                table: "StorageImportDetail");

            migrationBuilder.DropColumn(
                name: "Output",
                table: "StorageImport");

            migrationBuilder.DropColumn(
                name: "FabricPurchaseOrderNumber",
                table: "IssuedLine");
        }
    }
}
