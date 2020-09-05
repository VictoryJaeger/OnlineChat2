using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalROnlineChatServer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace SignalROnlineChatServer.DataBase
{
    public class OnlineChatDBContext : IdentityDbContext<User>
    {
        public OnlineChatDBContext(DbContextOptions<OnlineChatDBContext> options) : base(options) { }
        
        public DbSet<Chat> Chats { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<ChatUser> ChatUsers { get; set; }
        public DbSet<Connection> Connections { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                .HasOne(c => c.Chat)
                .WithMany(m => m.Messages)
                .HasForeignKey(x => x.ChatId);

            builder.Entity<ChatUser>()
                .HasKey(x => new { x.ChatId, x.UserId });
        }
    }
}
