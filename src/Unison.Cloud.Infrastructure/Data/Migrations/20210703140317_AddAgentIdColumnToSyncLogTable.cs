using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class AddAgentIdColumnToSyncLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentId",
                table: "SyncLog",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentId",
                table: "SyncLog");
        }
    }
}
