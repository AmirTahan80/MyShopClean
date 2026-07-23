using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class EnterpriseSecurityHardening : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF EXISTS (SELECT 1 FROM Factors GROUP BY CartId HAVING COUNT(*) > 1)
                    THROW 51000, 'Duplicate invoices exist for the same cart. Resolve them before applying EnterpriseSecurityHardening.', 1;

                IF EXISTS (SELECT 1 FROM Discounts GROUP BY CodeName HAVING COUNT(*) > 1)
                    THROW 51001, 'Duplicate discount codes exist. Resolve them before applying EnterpriseSecurityHardening.', 1;

                IF EXISTS (SELECT 1 FROM Discounts WHERE LEN(CodeName) > 450)
                    THROW 51002, 'A discount code exceeds the supported length of 450 characters.', 1;
                """);

            migrationBuilder.DropIndex(
                name: "IX_RequestPays_ApplicationUserId",
                table: "RequestPays");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId",
                table: "Carts");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "RequestPays",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Products",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CodeName",
                table: "Discounts",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "AttributeTemplates",
                type: "rowversion",
                rowVersion: true,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestPays_ApplicationUserId_IsPay",
                table: "RequestPays",
                columns: new[] { "ApplicationUserId", "IsPay" });

            migrationBuilder.CreateIndex(
                name: "IX_Factors_CartId",
                table: "Factors",
                column: "CartId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Discounts_CodeName",
                table: "Discounts",
                column: "CodeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId_IsFinally",
                table: "Carts",
                columns: new[] { "UserId", "IsFinally" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestPays_ApplicationUserId_IsPay",
                table: "RequestPays");

            migrationBuilder.DropIndex(
                name: "IX_Factors_CartId",
                table: "Factors");

            migrationBuilder.DropIndex(
                name: "IX_Discounts_CodeName",
                table: "Discounts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_UserId_IsFinally",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "RequestPays");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "AttributeTemplates");

            migrationBuilder.AlterColumn<string>(
                name: "CodeName",
                table: "Discounts",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateIndex(
                name: "IX_RequestPays_ApplicationUserId",
                table: "RequestPays",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserId",
                table: "Carts",
                column: "UserId");
        }
    }
}
