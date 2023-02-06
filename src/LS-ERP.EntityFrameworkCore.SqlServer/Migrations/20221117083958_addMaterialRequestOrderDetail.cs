using Microsoft.EntityFrameworkCore.Migrations;

namespace LS_ERP.EntityFrameworkCore.SqlServer.Migrations
{
    public partial class addMaterialRequestOrderDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MaterialRequestOrderDetails",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaterialRequestId = table.Column<int>(type: "int", nullable: false),
                    ItemStyleNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderDetailID = table.Column<int>(type: "int", nullable: false),
                    CustomerStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LSStyle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ColorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GarmentSize = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    SampleQuantity = table.Column<int>(type: "int", nullable: false),
                    PercentQuantity = table.Column<int>(type: "int", nullable: false),
                    TotalQuantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialRequestOrderDetails", x => x.ID);
                    table.ForeignKey(
                        name: "FK_MaterialRequestOrderDetails_MaterialRequests_MaterialRequestId",
                        column: x => x.MaterialRequestId,
                        principalTable: "MaterialRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MaterialRequestOrderDetails_MaterialRequestId",
                table: "MaterialRequestOrderDetails",
                column: "MaterialRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MaterialRequestOrderDetails");
        }
    }
}
