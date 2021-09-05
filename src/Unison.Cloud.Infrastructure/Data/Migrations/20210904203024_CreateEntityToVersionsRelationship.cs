using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class CreateEntityToVersionsRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncVersions_SyncEntities_SyncEntityId",
                table: "SyncVersions");

            migrationBuilder.DropIndex(
                name: "IX_SyncVersions_SyncEntityId",
                table: "SyncVersions");

            migrationBuilder.DropColumn(
                name: "SyncEntityId",
                table: "SyncVersions");

            migrationBuilder.CreateIndex(
                name: "IX_SyncVersions_AgentId",
                table: "SyncVersions",
                column: "AgentId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncVersions_EntityId",
                table: "SyncVersions",
                column: "EntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncVersions_SyncAgent_AgentId",
                table: "SyncVersions",
                column: "AgentId",
                principalTable: "SyncAgent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SyncVersions_SyncEntities_EntityId",
                table: "SyncVersions",
                column: "EntityId",
                principalTable: "SyncEntities",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncVersions_SyncAgent_AgentId",
                table: "SyncVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_SyncVersions_SyncEntities_EntityId",
                table: "SyncVersions");

            migrationBuilder.DropIndex(
                name: "IX_SyncVersions_AgentId",
                table: "SyncVersions");

            migrationBuilder.DropIndex(
                name: "IX_SyncVersions_EntityId",
                table: "SyncVersions");

            migrationBuilder.AddColumn<int>(
                name: "SyncEntityId",
                table: "SyncVersions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncVersions_SyncEntityId",
                table: "SyncVersions",
                column: "SyncEntityId");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncVersions_SyncEntities_SyncEntityId",
                table: "SyncVersions",
                column: "SyncEntityId",
                principalTable: "SyncEntities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
