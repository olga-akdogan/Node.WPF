using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Common;
using Node.ModelLibrary.Identity;
using Node.ModelLibrary.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Node.ModelLibrary.Data
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<BirthLocation> BirthLocations => Set<BirthLocation>();
        public DbSet<NatalChart> NatalCharts => Set<NatalChart>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            
            builder.Entity<Profile>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<BirthLocation>().HasQueryFilter(e => !e.IsDeleted);
            builder.Entity<NatalChart>().HasQueryFilter(e => !e.IsDeleted);

           
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

            ConfigureBaseEntityDefaults(builder);
        }

        private static void ConfigureBaseEntityDefaults(ModelBuilder builder)
        {
            builder.Entity<Profile>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<BirthLocation>().Property(e => e.IsDeleted).HasDefaultValue(false);
            builder.Entity<NatalChart>().Property(e => e.IsDeleted).HasDefaultValue(false);
        }

        public override int SaveChanges()
        {
            ApplySoftDelete();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplySoftDelete();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplySoftDelete()
        {
            foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
            {
                if (entry.State == EntityState.Deleted)
                {
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.DeleteAt = DateTime.UtcNow; 
                }
            }
        }
    }
}