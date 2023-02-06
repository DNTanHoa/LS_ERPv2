using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class createDeliveryNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryNote",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VenderID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSend = table.Column<bool>(type: "bit", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryNote", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryNoteDetail",
                columns: table => new
                {
                    ID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CuttingCardID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MergeBlockLSStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FabricContrastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Set = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WorkCenterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TalbleNO = table.Column<int>(type: "int", nullable: true),
                    TotalPackage = table.Column<int>(type: "int", nullable: true),
                    ProduceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AllocQuantity = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsSend = table.Column<bool>(type: "bit", nullable: false),
                    SendDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsReceived = table.Column<bool>(type: "bit", nullable: false),
                    ReceivedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryNoteDetail", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryNote");

            migrationBuilder.DropTable(
                name: "DeliveryNoteDetail");
        }
    }
}
