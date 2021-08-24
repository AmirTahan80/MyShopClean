using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class ChangeSomthingInRequestPay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestPays",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DatePay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    IsPay = table.Column<bool>(type: "bit", nullable: false),
                    RefId = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CartId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestPays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestPays_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RequestPays_Carts_CartId",
                        column: x => x.CartId,
                        principalTable: "Carts",
                        principalColumn: "CartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestPays_ApplicationUserId",
                table: "RequestPays",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestPays_CartId",
                table: "RequestPays",
                column: "CartId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestPays");
        }
    }
}
