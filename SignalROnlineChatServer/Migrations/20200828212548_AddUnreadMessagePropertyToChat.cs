using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalROnlineChatServer.Migrations
{
    public partial class AddUnreadMessagePropertyToChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnreadMessages",
                table: "Chats",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnreadMessages",
                table: "Chats");
        }
    }
}
