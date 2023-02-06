using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addGarmentColorCodeNetWeight : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InventoryPeriodEntry_InventoryPeriod_InventoryPeriodID",
            //    table: "InventoryPeriodEntry");

            //migrationBuilder.DropUniqueConstraint(
            //    name: "AK_InventoryPeriod_TempId",
            //    table: "InventoryPeriod");

            //migrationBuilder.DropColumn(
            //    name: "TempId",
            //    table: "InventoryPeriod");

            migrationBuilder.AddColumn<string>(
                name: "GarmentColorCode",
                table: "StyleNetWeight",
                type: "nvarchar(max)",
                nullable: true);

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

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InventoryPeriodEntry_InventoryPeriod_InventoryPeriodID",
            //    table: "InventoryPeriodEntry",
            //    column: "InventoryPeriodID",
            //    principalTable: "InventoryPeriod",
            //    principalColumn: "ID",
            //    onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_InventoryPeriodEntry_InventoryPeriod_InventoryPeriodID",
            //    table: "InventoryPeriodEntry");

            migrationBuilder.DropColumn(
                name: "GarmentColorCode",
                table: "StyleNetWeight");

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

            //migrationBuilder.AddColumn<int>(
            //    name: "TempId",
            //    table: "InventoryPeriod",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddUniqueConstraint(
            //    name: "AK_InventoryPeriod_TempId",
            //    table: "InventoryPeriod",
            //    column: "TempId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_InventoryPeriodEntry_InventoryPeriod_InventoryPeriodID",
            //    table: "InventoryPeriodEntry",
            //    column: "InventoryPeriodID",
            //    principalTable: "InventoryPeriod",
            //    principalColumn: "TempId",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
