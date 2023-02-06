using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class itemModelIFG : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Barcode",
                table: "ItemModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LSStyle",
                table: "ItemModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "ItemModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LabelCode",
                table: "ItemModel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MasterBox",
                table: "ItemModel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StyleDescription",
                table: "ItemModel",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Barcode",
                table: "ItemModel");

            migrationBuilder.DropColumn(
                name: "LSStyle",
                table: "ItemModel");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "ItemModel");

            migrationBuilder.DropColumn(
                name: "LabelCode",
                table: "ItemModel");

            migrationBuilder.DropColumn(
                name: "MasterBox",
                table: "ItemModel");

            migrationBuilder.DropColumn(
                name: "StyleDescription",
                table: "ItemModel");
        }
    }
}
