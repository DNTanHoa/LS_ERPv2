using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addTableSeparationPKL : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PackingListID",
                table: "ShippingPlanDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "PartPrice",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EffectiveDate",
                table: "PartPrice",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<bool>(
                name: "IsSeparated",
                table: "PackingList",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PPCBookDate",
                table: "PackingList",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SeparationPackingList",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PackingListID = table.Column<int>(type: "int", nullable: false),
                    CustomerID = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    TotalQuantity = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    SeparateOrdinal = table.Column<int>(type: "int", nullable: true),
                    InvoiceID = table.Column<long>(type: "bigint", nullable: true),
                    IsShipped = table.Column<bool>(type: "bit", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeparationPackingList", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SeparationPackingList_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeparationPackingList_PackingList_PackingListID",
                        column: x => x.PackingListID,
                        principalTable: "PackingList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SeparationPackingLine",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SequenceNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LSStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    QuantitySize = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    QuantityPerPackage = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    PackagesPerBox = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    QuantityPerCarton = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    TotalQuantity = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    NetWeight = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    GrossWeight = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrePack = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    SeparationPackingListID = table.Column<int>(type: "int", nullable: false),
                    BoxDimensionCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Length = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    Width = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    Height = table.Column<decimal>(type: "DECIMAL(19,9)", nullable: true),
                    FromNo = table.Column<int>(type: "int", nullable: true),
                    ToNo = table.Column<int>(type: "int", nullable: true),
                    TotalCarton = table.Column<int>(type: "int", nullable: true),
                    DeliveryPlace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeparationPackingLine", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SeparationPackingLine_BoxDimension_BoxDimensionCode",
                        column: x => x.BoxDimensionCode,
                        principalTable: "BoxDimension",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SeparationPackingLine_SeparationPackingList_SeparationPackingListID",
                        column: x => x.SeparationPackingListID,
                        principalTable: "SeparationPackingList",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SeparationPackingLine_BoxDimensionCode",
                table: "SeparationPackingLine",
                column: "BoxDimensionCode");

            migrationBuilder.CreateIndex(
                name: "IX_SeparationPackingLine_SeparationPackingListID",
                table: "SeparationPackingLine",
                column: "SeparationPackingListID");

            migrationBuilder.CreateIndex(
                name: "IX_SeparationPackingList_CustomerID",
                table: "SeparationPackingList",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_SeparationPackingList_PackingListID",
                table: "SeparationPackingList",
                column: "PackingListID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SeparationPackingLine");

            migrationBuilder.DropTable(
                name: "SeparationPackingList");

            migrationBuilder.DropColumn(
                name: "PackingListID",
                table: "ShippingPlanDetails");

            migrationBuilder.DropColumn(
                name: "IsSeparated",
                table: "PackingList");

            migrationBuilder.DropColumn(
                name: "PPCBookDate",
                table: "PackingList");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ExpiryDate",
                table: "PartPrice",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EffectiveDate",
                table: "PartPrice",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
