using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class Create_Adjective_Value_SubValue_ForProductAndSetRelationShip : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Adjective",
                columns: table => new
                {
                    AdjectiveId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdjectiveName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adjective", x => x.AdjectiveId);
                    table.ForeignKey(
                        name: "FK_Adjective_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Value",
                columns: table => new
                {
                    ValueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ValueName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValuePrice = table.Column<int>(type: "int", nullable: false),
                    ValueCount = table.Column<int>(type: "int", nullable: false),
                    AdjectiveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Value", x => x.ValueId);
                    table.ForeignKey(
                        name: "FK_Value_Adjective_AdjectiveId",
                        column: x => x.AdjectiveId,
                        principalTable: "Adjective",
                        principalColumn: "AdjectiveId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubValue",
                columns: table => new
                {
                    SubValueId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubValueName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Valuetype = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ValueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubValue", x => x.SubValueId);
                    table.ForeignKey(
                        name: "FK_SubValue_Value_ValueId",
                        column: x => x.ValueId,
                        principalTable: "Value",
                        principalColumn: "ValueId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adjective_ProductId",
                table: "Adjective",
                column: "ProductId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubValue_ValueId",
                table: "SubValue",
                column: "ValueId");

            migrationBuilder.CreateIndex(
                name: "IX_Value_AdjectiveId",
                table: "Value",
                column: "AdjectiveId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubValue");

            migrationBuilder.DropTable(
                name: "Value");

            migrationBuilder.DropTable(
                name: "Adjective");
        }
    }
}
