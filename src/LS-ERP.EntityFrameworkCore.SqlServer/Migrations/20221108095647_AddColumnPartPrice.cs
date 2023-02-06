using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class AddColumnPartPrice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerID",
                table: "PartPrice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GarmentColorCode",
                table: "PartPrice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GarmentSize",
                table: "PartPrice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Operation",
                table: "PartPrice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "PartPrice",
                type: "DECIMAL(19,9)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionType",
                table: "PartPrice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "PartPrice",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OtherPrice",
                table: "OrderDetail",
                type: "DECIMAL(19,9)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isConfirmed",
                table: "Invoice",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerID",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "GarmentColorCode",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "GarmentSize",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "Operation",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "ProductionType",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "PartPrice");

            migrationBuilder.DropColumn(
                name: "OtherPrice",
                table: "OrderDetail");

            migrationBuilder.DropColumn(
                name: "isConfirmed",
                table: "Invoice");
        }
    }
}
