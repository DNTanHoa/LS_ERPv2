using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addColumnIsRevisedPKL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeparationPackingList_PackingList_PackingListID",
                table: "SeparationPackingList");

            migrationBuilder.DropIndex(
                name: "IX_SeparationPackingList_PackingListID",
                table: "SeparationPackingList");

            migrationBuilder.AddColumn<bool>(
                name: "IsRevised",
                table: "PackingList",
                type: "bit",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRevised",
                table: "PackingList");

            migrationBuilder.CreateIndex(
                name: "IX_SeparationPackingList_PackingListID",
                table: "SeparationPackingList",
                column: "PackingListID");

            migrationBuilder.AddForeignKey(
                name: "FK_SeparationPackingList_PackingList_PackingListID",
                table: "SeparationPackingList",
                column: "PackingListID",
                principalTable: "PackingList",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
