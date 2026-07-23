using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infra.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSiteSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SiteSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SiteName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SiteTagline = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PrimaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    SecondaryColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    AccentColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    FooterTitle = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    FooterDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FooterCopyright = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SupportEmail = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    PublicBaseUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TorobEnabled = table.Column<bool>(type: "bit", nullable: false),
                    TorobAccessToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    EmallsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    EmallsAccessToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSettings", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SiteSettings");
        }
    }
}
