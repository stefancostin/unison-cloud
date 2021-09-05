using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class CreateVersionsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Version",
                table: "SyncEntities");

            migrationBuilder.CreateTable(
                name: "SyncVersions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    AgentId = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false, defaultValue: 1L),
                    SyncEntityId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SyncVersions_SyncEntities_SyncEntityId",
                        column: x => x.SyncEntityId,
                        principalTable: "SyncEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncVersions_SyncEntityId",
                table: "SyncVersions",
                column: "SyncEntityId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncVersions");

            migrationBuilder.AddColumn<long>(
                name: "Version",
                table: "SyncEntities",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);
        }
    }
}
