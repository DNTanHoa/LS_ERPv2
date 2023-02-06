using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class ChangeSizeBoxWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BoxWeight",
                table: "StyleNetWeight",
                type: "decimal(19,9)",
                precision: 19,
                scale: 9,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BoxWeight",
                table: "StyleNetWeight",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,9)",
                oldPrecision: 19,
                oldScale: 9,
                oldNullable: true);
        }
    }
}
