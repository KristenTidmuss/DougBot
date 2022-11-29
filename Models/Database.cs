using Microsoft.EntityFrameworkCore;

namespace DougBot.Models;

public class Database
{
    public class DougBotContext : DbContext
    {
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Queue> Queues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql("Host=192.168.1.252;Database=DougBot;Username=postgres;Password=88PVVel6QHqk");
    }
}