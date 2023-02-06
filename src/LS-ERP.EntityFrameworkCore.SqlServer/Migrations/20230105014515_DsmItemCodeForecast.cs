using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class DsmItemCodeForecast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "ItemMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DsmItemID",
                table: "ForecastMaterial",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "ItemMaster");

            migrationBuilder.DropColumn(
                name: "DsmItemID",
                table: "ForecastMaterial");
        }
    }
}
