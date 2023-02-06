using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class AddColumnPartMaster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Component",
                table: "PartMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EAN",
                table: "PartMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Label",
                table: "PartMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelLib",
                table: "PartMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModelLibProd",
                table: "PartMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PCB",
                table: "PartMaster",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "PartMaster",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UE",
                table: "PartMaster",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Component",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "EAN",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "Label",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "ModelLib",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "ModelLibProd",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "PCB",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "PartMaster");

            migrationBuilder.DropColumn(
                name: "UE",
                table: "PartMaster");
        }
    }
}
