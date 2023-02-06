using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class fileNameBOM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FileName",
                table: "PartRevision",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileNameServer",
                table: "PartRevision",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "PartRevision",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileName",
                table: "PartRevision");

            migrationBuilder.DropColumn(
                name: "FileNameServer",
                table: "PartRevision");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "PartRevision");
        }
    }
}
