using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateReceiptLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptLine_PurchaseOrderLine_PurchaseOrderLineID1",
                table: "ReceiptLine");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptLine_PurchaseOrderLineID1",
                table: "ReceiptLine");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderLineID1",
                table: "ReceiptLine");

            migrationBuilder.AlterColumn<long>(
                name: "PurchaseOrderLineID",
                table: "ReceiptLine",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptLine_PurchaseOrderLineID",
                table: "ReceiptLine",
                column: "PurchaseOrderLineID");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptLine_PurchaseOrderLine_PurchaseOrderLineID",
                table: "ReceiptLine",
                column: "PurchaseOrderLineID",
                principalTable: "PurchaseOrderLine",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReceiptLine_PurchaseOrderLine_PurchaseOrderLineID",
                table: "ReceiptLine");

            migrationBuilder.DropIndex(
                name: "IX_ReceiptLine_PurchaseOrderLineID",
                table: "ReceiptLine");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaseOrderLineID",
                table: "ReceiptLine",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddColumn<long>(
                name: "PurchaseOrderLineID1",
                table: "ReceiptLine",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptLine_PurchaseOrderLineID1",
                table: "ReceiptLine",
                column: "PurchaseOrderLineID1");

            migrationBuilder.AddForeignKey(
                name: "FK_ReceiptLine_PurchaseOrderLine_PurchaseOrderLineID1",
                table: "ReceiptLine",
                column: "PurchaseOrderLineID1",
                principalTable: "PurchaseOrderLine",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
