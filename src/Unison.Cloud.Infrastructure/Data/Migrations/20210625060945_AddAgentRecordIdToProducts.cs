using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class AddAgentRecordIdToProducts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AgentRecordId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AgentRecordId",
                table: "Products");
        }
    }
}
