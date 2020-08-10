using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalROnlineChatServer.Migrations
{
    public partial class DeleteUserAgentProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "Connections");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "Connections",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
