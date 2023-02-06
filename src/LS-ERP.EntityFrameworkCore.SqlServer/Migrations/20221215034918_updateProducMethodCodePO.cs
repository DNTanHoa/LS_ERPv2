using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateProducMethodCodePO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductionMethodCode",
                table: "PurchaseOrder",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrder_ProductionMethodCode",
                table: "PurchaseOrder",
                column: "ProductionMethodCode");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrder_PriceTerm_ProductionMethodCode",
                table: "PurchaseOrder",
                column: "ProductionMethodCode",
                principalTable: "PriceTerm",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrder_PriceTerm_ProductionMethodCode",
                table: "PurchaseOrder");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrder_ProductionMethodCode",
                table: "PurchaseOrder");

            migrationBuilder.DropColumn(
                name: "ProductionMethodCode",
                table: "PurchaseOrder");
        }
    }
}
