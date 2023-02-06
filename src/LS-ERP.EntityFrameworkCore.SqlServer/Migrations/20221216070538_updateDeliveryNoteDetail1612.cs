using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateDeliveryNoteDetail1612 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TalbleNO",
                table: "DeliveryNoteDetail",
                newName: "TableNO");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TableNO",
                table: "DeliveryNoteDetail",
                newName: "TalbleNO");
        }
    }
}
