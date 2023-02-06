using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class changeColumnInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isConfirmed",
                table: "Invoice",
                newName: "IsConfirmed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsConfirmed",
                table: "Invoice",
                newName: "isConfirmed");
        }
    }
}
