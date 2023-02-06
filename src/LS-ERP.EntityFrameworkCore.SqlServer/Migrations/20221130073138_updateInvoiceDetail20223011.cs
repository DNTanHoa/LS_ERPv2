using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateInvoiceDetail20223011 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PriceCM",
                table: "InvoiceDetail",
                type: "DECIMAL(19,9)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceFOB",
                table: "InvoiceDetail",
                type: "DECIMAL(19,9)",
                nullable: true);

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceCM",
                table: "InvoiceDetail");

            migrationBuilder.DropColumn(
                name: "PriceFOB",
                table: "InvoiceDetail");

           
        }
    }
}
