using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ValenceHub.Persistence.Read.Migrations
{
    public partial class RenameUserAuditLogsToAuditLogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing PK, rename table, and recreate PK with new name
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAuditLogs",
                table: "UserAuditLogs");

            migrationBuilder.RenameTable(
                name: "UserAuditLogs",
                newName: "AuditLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "UserAuditLogs");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAuditLogs",
                table: "UserAuditLogs",
                column: "Id");
        }
    }
}
