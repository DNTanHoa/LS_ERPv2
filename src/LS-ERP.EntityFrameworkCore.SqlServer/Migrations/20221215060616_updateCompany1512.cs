using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateCompany1512 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "DeliveryNoteDetail",
                newName: "DeliveryNoteID");

            migrationBuilder.RenameColumn(
                name: "DeliveryID",
                table: "DeliveryNoteDetail",
                newName: "CardType");

            migrationBuilder.AddColumn<string>(
                name: "OrtherName",
                table: "Company",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrtherName",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "DeliveryNoteID",
                table: "DeliveryNoteDetail",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "CardType",
                table: "DeliveryNoteDetail",
                newName: "DeliveryID");
        }
    }
}
