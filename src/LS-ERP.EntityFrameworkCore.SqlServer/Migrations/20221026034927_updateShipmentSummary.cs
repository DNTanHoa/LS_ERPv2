using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateShipmentSummary : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unit",
                table: "ShipmentSummary");

            migrationBuilder.AddColumn<string>(
                name: "UnitID",
                table: "ShipmentSummary",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentSummary_UnitID",
                table: "ShipmentSummary",
                column: "UnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShipmentSummary_Unit_UnitID",
                table: "ShipmentSummary",
                column: "UnitID",
                principalTable: "Unit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShipmentSummary_Unit_UnitID",
                table: "ShipmentSummary");

            migrationBuilder.DropIndex(
                name: "IX_ShipmentSummary_UnitID",
                table: "ShipmentSummary");

            migrationBuilder.DropColumn(
                name: "UnitID",
                table: "ShipmentSummary");

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "ShipmentSummary",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
