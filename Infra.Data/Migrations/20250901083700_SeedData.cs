using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infra.Data.Migrations
{
    public partial class SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Discriminator", "Name", "NormalizedName", "RoleNamePersian" },
                values: new object[,]
                {
                    { "1", "7c35e1b8-52a7-4dd5-ad2c-85d80c9978a5", "RoleModel", "Admin", "ADMIN", "مدیر سایت" },
                    { "2", "4b5e26f2-92ee-4900-8912-9827e21dbb43", "RoleModel", "User", "USER", "کاربر عادی" },
                    { "3", "be2ba07e-49ec-4da0-a5ee-fb6e84b1208c", "RoleModel", "Seller", "SELLER", "فروشنده" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Discriminator", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RegisterTime", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "1", 0, "1", "ApplicationUser", "admin@myshop.com", true, false, null, "ADMIN@MYSHOP.COM", "ADMIN@MYSHOP.COM", "AQAAAAEAACcQAAAAECu1lCrlQRqO1qkmHFKYjaPhHACS5EgZHm4QKvoNx/v6j/gbBaCBVJ4+HsWhfPmJaQ==", "09123456789", true, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, "admin@myshop.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "1", "1" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1", "1" });

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1");
        }
    }
}
