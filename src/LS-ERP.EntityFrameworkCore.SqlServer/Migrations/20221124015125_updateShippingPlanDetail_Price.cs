using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class updateShippingPlanDetail_Price : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ContractDate",
                table: "ShippingPlanDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EstimatedSupplierHandOver",
                table: "ShippingPlanDetails",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceCM",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceFOB",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ProductionDescription",
                table: "ShippingPlanDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPriceCM",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPriceFOB",
                table: "ShippingPlanDetails",
                type: "DECIMAL(19,9)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContractDate",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "EstimatedSupplierHandOver",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "PriceCM",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "PriceFOB",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "ProductionDescription",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "TotalPriceCM",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "TotalPriceFOB",
                table: "ShippingPlanDetails");
        }
    }
}
