using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateRemarkForCuttingCard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OldImport",
                table: "MaterialTransaction",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remark",
                table: "CuttingCard",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OldImport",
                table: "MaterialTransaction");

            migrationBuilder.DropColumn(
                name: "Remark",
                table: "CuttingCard");
        }
    }
}
