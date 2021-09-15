using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class UpdateRequestPayAddTrakId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IdReturnIdPay",
                table: "RequestPays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ReturnLinkIdPay",
                table: "RequestPays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdReturnIdPay",
                table: "RequestPays");

            migrationBuilder.DropColumn(
                name: "ReturnLinkIdPay",
                table: "RequestPays");
        }
    }
}
