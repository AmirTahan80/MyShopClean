using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class UpdateCartDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImgSrc",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "ProductPrice",
                table: "CartDetails",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "CartDetails",
                newName: "AttributesValue");

            migrationBuilder.AddColumn<int>(
                name: "TemplatesAttributeTemplateId",
                table: "CartDetails",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartDetails_TemplatesAttributeTemplateId",
                table: "CartDetails",
                column: "TemplatesAttributeTemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartDetails_AttributeTemplates_TemplatesAttributeTemplateId",
                table: "CartDetails",
                column: "TemplatesAttributeTemplateId",
                principalTable: "AttributeTemplates",
                principalColumn: "AttributeTemplateId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartDetails_AttributeTemplates_TemplatesAttributeTemplateId",
                table: "CartDetails");

            migrationBuilder.DropIndex(
                name: "IX_CartDetails_TemplatesAttributeTemplateId",
                table: "CartDetails");

            migrationBuilder.DropColumn(
                name: "TemplatesAttributeTemplateId",
                table: "CartDetails");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "CartDetails",
                newName: "ProductPrice");

            migrationBuilder.RenameColumn(
                name: "AttributesValue",
                table: "CartDetails",
                newName: "ProductName");

            migrationBuilder.AddColumn<string>(
                name: "ImgSrc",
                table: "CartDetails",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
