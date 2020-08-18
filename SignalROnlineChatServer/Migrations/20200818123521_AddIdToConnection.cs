using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalROnlineChatServer.Migrations
{
    public partial class AddIdToConnection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Connections",
                table: "Connections");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionID",
                table: "Connections",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Connections",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Connections",
                table: "Connections",
                column: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Connections",
                table: "Connections");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Connections");

            migrationBuilder.AlterColumn<string>(
                name: "ConnectionID",
                table: "Connections",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Connections",
                table: "Connections",
                column: "ConnectionID");
        }
    }
}
