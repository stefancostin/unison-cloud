using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class AddErrorMessageColummnToLogsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "SyncLog",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "SyncLog");
        }
    }
}
