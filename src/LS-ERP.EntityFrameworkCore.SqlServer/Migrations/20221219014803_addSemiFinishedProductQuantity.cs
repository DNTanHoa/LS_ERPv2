using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addSemiFinishedProductQuantity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SemiFinishedProductQuantity",
                table: "FabricRequestDetailLog",
                type: "DECIMAL(19,9)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SemiFinishedProductQuantity",
                table: "FabricRequestDetail",
                type: "DECIMAL(19,9)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SemiFinishedProductQuantity",
                table: "FabricRequestDetailLog");

            migrationBuilder.DropColumn(
                name: "SemiFinishedProductQuantity",
                table: "FabricRequestDetail");
        }
    }
}
