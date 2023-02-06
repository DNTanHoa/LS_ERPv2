using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateShu : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<string>(
            //    name: "OutputOrder",
            //    table: "StorageImportDetail",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ProductionMethodCode",
            //    table: "StorageImportDetail",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AddColumn<decimal>(
            //    name: "RollNo",
            //    table: "StorageImportDetail",
            //    type: "decimal(18,2)",
            //    nullable: false,
            //    defaultValue: 0m);

            //migrationBuilder.AddColumn<long>(
            //    name: "StorageDetailID",
            //    table: "StorageImportDetail",
            //    type: "bigint",
            //    nullable: false,
            //    defaultValue: 0L);

            //migrationBuilder.AddColumn<bool>(
            //    name: "Output",
            //    table: "StorageImport",
            //    type: "bit",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "FabricPurchaseOrderNumber",
            //    table: "IssuedLine",
            //    type: "nvarchar(max)",
            //    nullable: true);

            //migrationBuilder.AlterColumn<long>(
            //    name: "InventoryPeriodID",
            //    table: "InventoryPeriodEntry",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "long");

            //migrationBuilder.AlterColumn<long>(
            //    name: "ID",
            //    table: "InventoryPeriod",
            //    type: "bigint",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "long")
            //    .Annotation("SqlServer:Identity", "1, 1")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.CreateTable(
                name: "Shus",
                columns: table => new
                {
                    BarCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CustomerStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GarmentSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PCB = table.Column<int>(type: "int", nullable: false),
                    UE = table.Column<int>(type: "int", nullable: false),
                    CartNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Shus", x => x.BarCode);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Shus");

            //migrationBuilder.DropColumn(
            //    name: "OutputOrder",
            //    table: "StorageImportDetail");

            //migrationBuilder.DropColumn(
            //    name: "ProductionMethodCode",
            //    table: "StorageImportDetail");

            //migrationBuilder.DropColumn(
            //    name: "RollNo",
            //    table: "StorageImportDetail");

            //migrationBuilder.DropColumn(
            //    name: "StorageDetailID",
            //    table: "StorageImportDetail");

            //migrationBuilder.DropColumn(
            //    name: "Output",
            //    table: "StorageImport");

            //migrationBuilder.DropColumn(
            //    name: "FabricPurchaseOrderNumber",
            //    table: "IssuedLine");

            //migrationBuilder.AlterColumn<long>(
            //    name: "InventoryPeriodID",
            //    table: "InventoryPeriodEntry",
            //    type: "long",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint");

            //migrationBuilder.AlterColumn<long>(
            //    name: "ID",
            //    table: "InventoryPeriod",
            //    type: "long",
            //    nullable: false,
            //    oldClrType: typeof(long),
            //    oldType: "bigint")
            //    .Annotation("SqlServer:Identity", "1, 1")
            //    .OldAnnotation("SqlServer:Identity", "1, 1");
        }
    }
}
