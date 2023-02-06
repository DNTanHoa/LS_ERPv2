using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class fixMaterialRequestDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Roll",
                table: "MaterialRequestDetails",
                type: "decimal(19,9)",
                precision: 19,
                scale: 9,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "RequiredQuantity",
                table: "MaterialRequestDetails",
                type: "decimal(19,9)",
                precision: 19,
                scale: 9,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityPerUnit",
                table: "MaterialRequestDetails",
                type: "decimal(19,9)",
                precision: 19,
                scale: 9,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "MaterialRequestDetails",
                type: "decimal(19,9)",
                precision: 19,
                scale: 9,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Roll",
                table: "MaterialRequestDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,9)",
                oldPrecision: 19,
                oldScale: 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "RequiredQuantity",
                table: "MaterialRequestDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,9)",
                oldPrecision: 19,
                oldScale: 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityPerUnit",
                table: "MaterialRequestDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,9)",
                oldPrecision: 19,
                oldScale: 9);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "MaterialRequestDetails",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(19,9)",
                oldPrecision: 19,
                oldScale: 9);
        }
    }
}
