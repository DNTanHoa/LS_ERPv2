using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class ChangeTypeInventoryPeriodID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<int>(
            //    name: "InventoryPeriodID",
            //    table: "InventoryPeriodEntry",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<int>(
            //    name: "ID",
            //    table: "InventoryPeriod",
            //    type: "int",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint")
            //    .Annotation("SqlServer:Identity", "1, 1")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<int>(
                name: "InventoryPeriodID",
                table: "InventoryDetailFG",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "InventoryPeriodID",
                table: "FinishGoodTransaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<long>(
            //    name: "InventoryPeriodID",
            //    table: "InventoryPeriodEntry",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            //migrationBuilder.AlterColumn<long>(
            //    name: "ID",
            //    table: "InventoryPeriod",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(int),
            //    oldType: "int")
            //    .Annotation("SqlServer:Identity", "1, 1")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<long>(
                name: "InventoryPeriodID",
                table: "InventoryDetailFG",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "InventoryPeriodID",
                table: "FinishGoodTransaction",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
