using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class fabricRequestHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FabricRequestHistory",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerID = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CustomerStyleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentWastage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FabricRequestID = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FabricRequestHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FabricRequestHistory_Company_CompanyCode",
                        column: x => x.CompanyCode,
                        principalTable: "Company",
                        principalColumn: "Code",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FabricRequestHistory_Customer_CustomerID",
                        column: x => x.CustomerID,
                        principalTable: "Customer",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FabricRequestHistory_FabricRequest_FabricRequestID",
                        column: x => x.FabricRequestID,
                        principalTable: "FabricRequest",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FabricRequestHistory_Status_StatusID",
                        column: x => x.StatusID,
                        principalTable: "Status",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FabricRequestDetailHistory",
                columns: table => new
                {
                    ID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FabricColor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    RequestQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ConsumtionQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StreakRequestQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BalanceQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CustomerConsumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CuttingConsumption = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ItemColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ItemMasterID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BreadthWidth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FabricRequestHistoryID = table.Column<long>(type: "bigint", nullable: false),
                    PercentPrint = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentPrintQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentWastage = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IssuedQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PercentWastageQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FabricRequestDetailHistory", x => x.ID);
                    table.ForeignKey(
                        name: "FK_FabricRequestDetailHistory_FabricRequestHistory_FabricRequestHistoryID",
                        column: x => x.FabricRequestHistoryID,
                        principalTable: "FabricRequestHistory",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FabricRequestDetailHistory_FabricRequestHistoryID",
                table: "FabricRequestDetailHistory",
                column: "FabricRequestHistoryID");

            migrationBuilder.CreateIndex(
                name: "IX_FabricRequestHistory_CompanyCode",
                table: "FabricRequestHistory",
                column: "CompanyCode");

            migrationBuilder.CreateIndex(
                name: "IX_FabricRequestHistory_CustomerID",
                table: "FabricRequestHistory",
                column: "CustomerID");

            migrationBuilder.CreateIndex(
                name: "IX_FabricRequestHistory_FabricRequestID",
                table: "FabricRequestHistory",
                column: "FabricRequestID");

            migrationBuilder.CreateIndex(
                name: "IX_FabricRequestHistory_StatusID",
                table: "FabricRequestHistory",
                column: "StatusID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FabricRequestDetailHistory");

            migrationBuilder.DropTable(
                name: "FabricRequestHistory");
        }
    }
}
