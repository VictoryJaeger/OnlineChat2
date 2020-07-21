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

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<ChatUser>().HasNoKey();

            //builder.Entity<ChatUser>()
            //    .HasIndex(x => x.ChatId)
            //    .IsUnique();

            //builder.Entity<ChatUser>()
            //    .HasIndex(x => x.UserId)
            //    .IsUnique();


            builder.Entity<Message>()
                .HasOne(c => c.Chat)
                .WithMany(m => m.Messages)
                .HasForeignKey(x => x.ChatId);

            //builder.Entity<ChatUser>()
            //   .HasOne<Chat>()
            //   .WithMany()
            //   .HasForeignKey(x => x.ChatId);

            //builder.Entity<ChatUser>()
            //   .HasOne<User>()
            //   .WithMany()
            //   .HasForeignKey(x => x.UserId);

            builder.Entity<ChatUser>()
                .HasKey(x => new { x.ChatId, x.UserId });
        }
    }
}
