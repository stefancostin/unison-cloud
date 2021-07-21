using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class AddAccountsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncLog_AgentId",
                table: "SyncLog",
                column: "AgentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncLog_SyncAgent_AgentId",
                table: "SyncLog",
                column: "AgentId",
                principalTable: "SyncAgent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncLog_SyncAgent_AgentId",
                table: "SyncLog");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropIndex(
                name: "IX_SyncLog_AgentId",
                table: "SyncLog");
        }
    }
}
