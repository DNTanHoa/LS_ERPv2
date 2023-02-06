using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateDeiveryNote1512 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VenderName",
                table: "DeliveryNote",
                newName: "VendorName");

            migrationBuilder.RenameColumn(
                name: "VenderID",
                table: "DeliveryNote",
                newName: "VendorID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VendorName",
                table: "DeliveryNote",
                newName: "VenderName");

            migrationBuilder.RenameColumn(
                name: "VendorID",
                table: "DeliveryNote",
                newName: "VenderID");
        }
    }
}
