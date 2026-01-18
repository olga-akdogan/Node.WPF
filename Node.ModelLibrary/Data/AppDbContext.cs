using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Identity;   
using Node.ModelLibrary.Models;
using Node.ModelLibrary.Common;
using System.Linq.Expressions;

namespace Node.ModelLibrary.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Profile> Profiles => Set<Profile>();
        public DbSet<Photo> Photos => Set<Photo>();
        public DbSet<Match> Matches => Set<Match>();
        public DbSet<Message> Messages => Set<Message>();   

        protected override void OnModelCreating (ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            foreach (var entityType in builder.Model.GetEntityTypes())

            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var parameter = Expression.Parameter(entityType.ClrType, "e");
                    var isDeletedProp = Expression.Property(parameter, nameof(BaseEntity.IsDeleted));
                    var compare = Expression.Equal(isDeletedProp, Expression.Constant(false));
                    var lambda = Expression.Lambda(compare, parameter);

                    builder.Entity(entityType.ClrType).HasQueryFilter(lambda);
                }
            }

           
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
    }
}
