using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Common;
using Node.ModelLibrary.Identity;
using Node.ModelLibrary.Models;

namespace Node.ModelLibrary.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Message> Messages => Set<Message>();
        public DbSet<BirthLocation> BirthLocations => Set<BirthLocation>();
        public DbSet<NatalChart> NatalCharts => Set<NatalChart>();
        public DbSet<Conversation> Conversations => Set<Conversation>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

           
            ConfigureBaseEntityDefaults(builder);

           
            builder.Entity<AppUser>()
                .HasOne(u => u.Profile)
                .WithOne(p => p.AppUser)
                .HasForeignKey<Profile>(p => p.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

          
            builder.Entity<Profile>()
                .HasIndex(p => p.AppUserId)
                .IsUnique();

         
            builder.Entity<Profile>()
                .HasOne(p => p.BirthLocation)
                .WithOne(bl => bl.Profile)
                .HasForeignKey<BirthLocation>(bl => bl.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

        
            builder.Entity<Profile>()
                .HasOne(p => p.NatalChart)
                .WithOne(c => c.Profile)
                .HasForeignKey<NatalChart>(c => c.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

          
            builder.Entity<Match>()
                .HasOne(m => m.Conversation)
                .WithOne(c => c.Match)
                .HasForeignKey<Conversation>(c => c.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

          
            builder.Entity<Message>()
                .HasOne(m => m.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(m => m.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

          
            builder.Entity<Match>()
                .HasOne(m => m.UserA)
                .WithMany()
                .HasForeignKey(m => m.UserAId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Match>()
                .HasOne(m => m.UserB)
                .WithMany()
                .HasForeignKey(m => m.UserBId)
                .OnDelete(DeleteBehavior.Restrict);

         
        }

        private static void ConfigureBaseEntityDefaults(ModelBuilder builder)
        {
          
            builder.Entity<Profile>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<BirthLocation>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<NatalChart>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<Photo>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<Match>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<Message>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<Conversation>().Property(e => e.IsDeleted).HasDefaultValue(false);

            
        }
    }
}