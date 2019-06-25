using System;
using Microsoft.EntityFrameworkCore;

namespace Starlemon.Models
{
    public class StarlemonContext : DbContext
    {
        public DbSet<Video> Videos { get; set; }

        public DbSet<VideoPage> VideoPages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(Environment.GetEnvironmentVariable("Starlemon_ConnectionString"));

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<VideoPageNode>()
                .HasKey(n => new { n.VideoPageId, n.NodeId });

            modelBuilder.Entity<VideoPage>()
                .HasAlternateKey(p => new { p.VideoId, p.PageId });
        }
    }
}
