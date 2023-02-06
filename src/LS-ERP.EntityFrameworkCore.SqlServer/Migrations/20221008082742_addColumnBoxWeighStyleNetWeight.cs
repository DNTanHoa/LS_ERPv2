using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addColumnBoxWeighStyleNetWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BoxDimensionCode",
                table: "StyleNetWeight",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "BoxWeight",
                table: "StyleNetWeight",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BoxDimensionCode",
                table: "StyleNetWeight");

            migrationBuilder.DropColumn(
                name: "BoxWeight",
                table: "StyleNetWeight");
        }
    }
}
