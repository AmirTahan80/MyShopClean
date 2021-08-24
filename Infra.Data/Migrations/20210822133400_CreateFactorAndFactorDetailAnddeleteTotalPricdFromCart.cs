using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class CreateFactorAndFactorDetailAnddeleteTotalPricdFromCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Carts");

            migrationBuilder.CreateTable(
                name: "Factors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CartId = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserFamilly = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Factors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Factors_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FactorDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductCount = table.Column<int>(type: "int", nullable: false),
                    ProductPrice = table.Column<int>(type: "int", nullable: false),
                    TotalPrice = table.Column<int>(type: "int", nullable: false),
                    AttributesName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AttributesValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageSrc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FactorId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactorDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FactorDetails_Factors_FactorId",
                        column: x => x.FactorId,
                        principalTable: "Factors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FactorDetails_FactorId",
                table: "FactorDetails",
                column: "FactorId");

            migrationBuilder.CreateIndex(
                name: "IX_Factors_UserId",
                table: "Factors",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FactorDetails");

            migrationBuilder.DropTable(
                name: "Factors");

            migrationBuilder.AddColumn<int>(
                name: "TotalPrice",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
