using Microsoft.EntityFrameworkCore;
using MusicHub.Data.Models;

namespace MusicHub.Data
{
    public class MusicHubDbContext : DbContext
    {
        public MusicHubDbContext()
        {
        }

        public MusicHubDbContext(Microsoft.EntityFrameworkCore.DbContextOptions options) 
            : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
        public DbSet<Performer> Performers { get; set; }
        public DbSet<Producer> Producers { get; set; }
        public DbSet<Song> Songs { get; set; }
        public DbSet<SongPerformer> SongsPerformers { get; set; }
        public DbSet<Writer> Writers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=MusicHub;Integrated Security=true");
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SongPerformer>(x => 
            {
                x.HasKey(x => new {x.SongId, x.PerformerId }); 
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
