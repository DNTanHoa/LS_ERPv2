using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addColumnHangerPKLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HangerCode",
                table: "PackingLine",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitID",
                table: "PackingLine",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PackingLine_HangerCode",
                table: "PackingLine",
                column: "HangerCode");

            migrationBuilder.CreateIndex(
                name: "IX_PackingLine_UnitID",
                table: "PackingLine",
                column: "UnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_PackingLine_Hanger_HangerCode",
                table: "PackingLine",
                column: "HangerCode",
                principalTable: "Hanger",
                principalColumn: "Code",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PackingLine_Unit_UnitID",
                table: "PackingLine",
                column: "UnitID",
                principalTable: "Unit",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PackingLine_Hanger_HangerCode",
                table: "PackingLine");

            migrationBuilder.DropForeignKey(
                name: "FK_PackingLine_Unit_UnitID",
                table: "PackingLine");

            migrationBuilder.DropIndex(
                name: "IX_PackingLine_HangerCode",
                table: "PackingLine");

            migrationBuilder.DropIndex(
                name: "IX_PackingLine_UnitID",
                table: "PackingLine");

            migrationBuilder.DropColumn(
                name: "HangerCode",
                table: "PackingLine");

            migrationBuilder.DropColumn(
                name: "UnitID",
                table: "PackingLine");
        }
    }
}
