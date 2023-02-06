using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class DsmItemIDFull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "StorageImportDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "StorageDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "ReceiptLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "ReceiptGroupLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "PurchaseRequestLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "PurchaseRequestGroupLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "PurchaseOrderLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "PurchaseOrderGroupLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "ProductionBOM",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "PartRevisionLogDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "MaterialTransaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "MaterialRequestDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "IssuedLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "IssuedGroupLine",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "StorageImportDetail");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "StorageDetail");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "ReceiptLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "ReceiptGroupLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "PurchaseRequestLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "PurchaseRequestGroupLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "PurchaseOrderLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "PurchaseOrderGroupLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "ProductionBOM");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "PartRevisionLogDetail");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "MaterialTransaction");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "MaterialRequestDetails");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "IssuedLine");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "IssuedGroupLine");
        }
    }
}
