using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class AddAgentNodeRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SyncEntities_NodeId",
                table: "SyncEntities",
                column: "NodeId");

            migrationBuilder.CreateIndex(
                name: "IX_SyncAgent_NodeId",
                table: "SyncAgent",
                column: "NodeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SyncAgent_SyncNodes_NodeId",
                table: "SyncAgent",
                column: "NodeId",
                principalTable: "SyncNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SyncEntities_SyncNodes_NodeId",
                table: "SyncEntities",
                column: "NodeId",
                principalTable: "SyncNodes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SyncAgent_SyncNodes_NodeId",
                table: "SyncAgent");

            migrationBuilder.DropForeignKey(
                name: "FK_SyncEntities_SyncNodes_NodeId",
                table: "SyncEntities");

            migrationBuilder.DropIndex(
                name: "IX_SyncEntities_NodeId",
                table: "SyncEntities");

            migrationBuilder.DropIndex(
                name: "IX_SyncAgent_NodeId",
                table: "SyncAgent");
        }
    }
}
