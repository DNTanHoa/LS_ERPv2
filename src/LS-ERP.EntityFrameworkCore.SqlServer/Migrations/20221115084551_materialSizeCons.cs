using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class materialSizeCons : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MaterialSizeConsumption",
                table: "PartMaterial",
                type: "DECIMAL(19,9)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "MaterialSizeConsumption",
                table: "PartMaterial",
                type: "decimal(18,2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "DECIMAL(19,9)",
                oldNullable: true);
        }
    }
}
