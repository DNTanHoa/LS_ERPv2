using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addColumnFullNameUnit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Unit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxInStockQuantity",
                table: "PartMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinInStockQuantity",
                table: "PartMaster",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Unit");

            migrationBuilder.DropColumn(
                name: "MaxInStockQuantity",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "MinInStockQuantity",
                table: "PartMaster");
        }
    }
}
