using Microsoft.EntityFrameworkCore.Migrations;

namespace Unison.Cloud.Infrastructure.Data.Migrations
{
    public partial class AddSyncAgentsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Version",
                table: "SyncEntities",
                type: "bigint",
                nullable: false,
                defaultValue: 1L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 0L);

            migrationBuilder.AlterColumn<int>(
                name: "NodeId",
                table: "SyncEntities",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SyncAgent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InstanceId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    NodeId = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncAgent", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncAgent_InstanceId",
                table: "SyncAgent",
                column: "InstanceId",
                unique: true,
                filter: "[InstanceId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncAgent");

            migrationBuilder.AlterColumn<long>(
                name: "Version",
                table: "SyncEntities",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1L);

            migrationBuilder.AlterColumn<int>(
                name: "NodeId",
                table: "SyncEntities",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);
        }
    }
}
