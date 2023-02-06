using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class fabricPODE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "IssuedLine",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PurchaseOrderNumber",
                table: "IssuedGroupLine",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "IssuedLine");

            migrationBuilder.DropColumn(
                name: "PurchaseOrderNumber",
                table: "IssuedGroupLine");
        }
    }
}
