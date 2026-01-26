using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Identity;
using Node.ModelLibrary.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Node.ModelLibrary.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(
            AppDbContext db,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            
            await db.Database.EnsureCreatedAsync();

            
            await EnsureRoleAsync(roleManager, "Admin");
            await EnsureRoleAsync(roleManager, "User");

            
            const string adminEmail = "admin@node.local";
            var admin = await EnsureUserAsync(
                userManager,
                email: adminEmail,
                displayName: "Admin",
                password: "Admin123!");

            await EnsureUserInRoleAsync(userManager, admin, "Admin");

            
            for (int i = 1; i <= 10; i++)
            {
                var email = $"user{i}@node.local";
                var user = await EnsureUserAsync(
                    userManager,
                    email: email,
                    displayName: $"User {i}",
                    password: "User123!");

                await EnsureUserInRoleAsync(userManager, user, "User");
            }

            
            var users = await userManager.Users
                .Where(u => u.Email != adminEmail)
                .OrderBy(u => u.Email)
                .Take(10)
                .ToListAsync();

            foreach (var u in users)
            {
               
                var existingProfile = await db.Profiles
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(p => p.AppUserId == u.Id);

                if (existingProfile != null)
                    continue;

                var profile = new Profile
                {
                    AppUserId = u.Id,
                    Bio = $"Hi, I am {u.DisplayName}.",
                    Hobbies = "Music, Travel, Coffee",

                    BirthPlace = "Brussels",
                    BirthLatitude = 50.8503,
                    BirthLongitude = 4.3517,

                    
                    BirthDateTimeUtc = new DateTime(1995, 01, 01, 12, 00, 00, DateTimeKind.Utc)
                        .AddDays(u.Email?.Length ?? 0),

                    BirthLocation = new BirthLocation
                    {
                        PlaceName = "Brussels",
                        Latitude = 50.8503,
                        Longitude = 4.3517
                    },

                    NatalChart = new NatalChart
                    {
                        ChartJson = "{\"demo\":true,\"note\":\"stub chart\"}",
                        SunLongitude = 120,
                        MoonLongitude = 200,
                        CreatedAtUtc = DateTime.UtcNow
                    }
                };

                db.Profiles.Add(profile);
            }

            await db.SaveChangesAsync();
        }

        private static async Task EnsureRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (await roleManager.RoleExistsAsync(roleName)) return;

            var result = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create role '{roleName}': {msg}");
            }
        }

        private static async Task<AppUser> EnsureUserAsync(
            UserManager<AppUser> userManager,
            string email,
            string displayName,
            string password)
        {
            var existing = await userManager.FindByEmailAsync(email);
            if (existing != null) return existing;

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                IsBlocked = false
            };

            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to create user '{email}': {msg}");
            }

            return user;
        }

        private static async Task EnsureUserInRoleAsync(
            UserManager<AppUser> userManager,
            AppUser user,
            string roleName)
        {
            if (await userManager.IsInRoleAsync(user, roleName)) return;

            var result = await userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
            {
                var msg = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to add '{user.Email}' to role '{roleName}': {msg}");
            }
        }
    }
}