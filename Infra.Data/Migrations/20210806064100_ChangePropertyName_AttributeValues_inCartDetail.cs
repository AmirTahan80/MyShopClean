using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class ChangePropertyName_AttributeValues_inCartDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttributesValue",
                table: "CartDetails",
                newName: "AttributeValues");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AttributeValues",
                table: "CartDetails",
                newName: "AttributesValue");
        }
    }
}
