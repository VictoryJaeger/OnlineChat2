using Microsoft.EntityFrameworkCore.Migrations;

namespace SignalROnlineChatServer.Migrations
{
    public partial class AddUnreadMessageCountToChatUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnreadMessages",
                table: "Chats");

            migrationBuilder.AddColumn<int>(
                name: "UnreadMessageCount",
                table: "ChatUsers",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnreadMessageCount",
                table: "ChatUsers");

            migrationBuilder.AddColumn<int>(
                name: "UnreadMessages",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
