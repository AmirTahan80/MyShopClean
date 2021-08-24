using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class UpdateFactorModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RefId",
                table: "Factors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DiscountFactor",
                columns: table => new
                {
                    DiscountsId = table.Column<int>(type: "int", nullable: false),
                    FactorsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountFactor", x => new { x.DiscountsId, x.FactorsId });
                    table.ForeignKey(
                        name: "FK_DiscountFactor_Discounts_DiscountsId",
                        column: x => x.DiscountsId,
                        principalTable: "Discounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountFactor_Factors_FactorsId",
                        column: x => x.FactorsId,
                        principalTable: "Factors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountFactor_FactorsId",
                table: "DiscountFactor",
                column: "FactorsId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountFactor");

            migrationBuilder.DropColumn(
                name: "RefId",
                table: "Factors");
        }
    }
}
