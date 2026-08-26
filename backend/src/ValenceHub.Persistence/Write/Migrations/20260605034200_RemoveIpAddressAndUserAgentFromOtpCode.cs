using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenceHub.Persistence.Write.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIpAddressAndUserAgentFromOtpCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "OtpCodes");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "OtpCodes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "OtpCodes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "OtpCodes",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
