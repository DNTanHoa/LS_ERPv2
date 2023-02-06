using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class ChangeColumnInventoryPeriodID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_InventoryDetailFG_InventoryFGID",
                table: "InventoryDetailFG",
                column: "InventoryFGID");

            migrationBuilder.CreateIndex(
                name: "IX_FinishGoodTransaction_InventoryFGID",
                table: "FinishGoodTransaction",
                column: "InventoryFGID");

            migrationBuilder.AddForeignKey(
                name: "FK_FinishGoodTransaction_InventoryFG_InventoryFGID",
                table: "FinishGoodTransaction",
                column: "InventoryFGID",
                principalTable: "InventoryFG",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InventoryDetailFG_InventoryFG_InventoryFGID",
                table: "InventoryDetailFG",
                column: "InventoryFGID",
                principalTable: "InventoryFG",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FinishGoodTransaction_InventoryFG_InventoryFGID",
                table: "FinishGoodTransaction");

            migrationBuilder.DropForeignKey(
                name: "FK_InventoryDetailFG_InventoryFG_InventoryFGID",
                table: "InventoryDetailFG");

            migrationBuilder.DropIndex(
                name: "IX_InventoryDetailFG_InventoryFGID",
                table: "InventoryDetailFG");

            migrationBuilder.DropIndex(
                name: "IX_FinishGoodTransaction_InventoryFGID",
                table: "FinishGoodTransaction");

        }
    }
}
