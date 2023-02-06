using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateShipment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ShipmentDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ShipmentDetail",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ShipmentDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedAt",
                table: "ShipmentDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastUpdatedBy",
                table: "ShipmentDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ShipmentDetail");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ShipmentDetail");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ShipmentDetail");

            migrationBuilder.DropColumn(
                name: "LastUpdatedAt",
                table: "ShipmentDetail");

            migrationBuilder.DropColumn(
                name: "LastUpdatedBy",
                table: "ShipmentDetail");
        }
    }
}
