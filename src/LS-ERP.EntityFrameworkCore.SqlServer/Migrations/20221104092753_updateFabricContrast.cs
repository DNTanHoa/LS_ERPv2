using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateFabricContrast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.AddColumn<string>(
                name: "DescriptionForPant",
                table: "FabricContrast",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DescriptionForShirt",
                table: "FabricContrast",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropColumn(
                name: "DescriptionForPant",
                table: "FabricContrast");

            migrationBuilder.DropColumn(
                name: "DescriptionForShirt",
                table: "FabricContrast");
        }
    }
}
