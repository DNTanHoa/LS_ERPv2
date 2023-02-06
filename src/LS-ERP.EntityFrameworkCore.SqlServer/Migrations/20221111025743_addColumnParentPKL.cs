using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addColumnParentPKL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ParentPackingListID",
                table: "PackingList",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeparatedOrdinal",
                table: "PackingList",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentPackingListID",
                table: "PackingList");

            migrationBuilder.DropColumn(
                name: "SeparatedOrdinal",
                table: "PackingList");
        }
    }
}
