using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Identity;
using Node.ModelLibrary.Models;

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


            string[] roles = { "Admin", "User" };
            foreach (var r in roles)
            {
                if (!await roleManager.RoleExistsAsync(r))
                    await roleManager.CreateAsync(new IdentityRole(r));
            }

          
            const string adminEmail = "admin@node.local";
            var admin = await userManager.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

            if (admin == null)
            {
                admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    DisplayName = "Admin",
                    IsBlocked = false
                };

                var createAdmin = await userManager.CreateAsync(admin, "Admin123!");
                if (!createAdmin.Succeeded)
                {
                    var msg = string.Join("; ", createAdmin.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Failed to create admin: {msg}");
                }

                await userManager.AddToRoleAsync(admin, "Admin");
            }

          
            var hasNonAdminUsers = await userManager.Users.AnyAsync(u => u.Email != adminEmail);
            if (!hasNonAdminUsers)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var email = $"user{i}@node.local";
                    var user = new AppUser
                    {
                        UserName = email,
                        Email = email,
                        DisplayName = $"User {i}",
                        IsBlocked = false
                    };

                    var createUser = await userManager.CreateAsync(user, "User123!");
                    if (!createUser.Succeeded) continue;

                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            
            var identityUsers = await userManager.Users
                .Where(u => u.Email != adminEmail)
                .OrderBy(u => u.Email)
                .Take(10)
                .ToListAsync();

            foreach (var u in identityUsers)
            {
                var hasProfile = await db.Profiles.AnyAsync(p => p.AppUserId == u.Id);
                if (hasProfile) continue;

                var profile = new Profile
                {
                    AppUserId = u.Id,
                    Bio = $"Hi, I am {u.DisplayName}.",
                    Hobbies = "Music, Travel, Coffee",

                    BirthDateTimeUtc = DateTime.UtcNow.AddYears(-20).AddDays(-u.Email!.Length),
                    BirthPlace = "Brussels",
                    BirthLatitude = 50.8503,
                    BirthLongitude = 4.3517,

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

            
            if (!await db.Matches.AnyAsync())
            {
                var demoUsers = await userManager.Users
                    .Where(u => u.Email != adminEmail)
                    .OrderBy(u => u.Email)
                    .Take(6)
                    .ToListAsync();

                if (demoUsers.Count >= 4)
                {
                    var m1 = new Match
                    {
                        UserAId = demoUsers[0].Id,
                        UserBId = demoUsers[1].Id,
                        Status = MatchStatus.Accepted,
                        CompatibilityScore = 82.5,
                        Conversation = new Conversation
                        {
                            CreatedAtUtc = DateTime.UtcNow,
                            Messages = new List<Message>
                            {
                                new Message { Content = "Hey! Nice to match 🙂", SenderId = demoUsers[0].Id },
                                new Message { Content = "Hi! Same here. How are you?", SenderId = demoUsers[1].Id }
                            }
                        }
                    };

                    var m2 = new Match
                    {
                        UserAId = demoUsers[2].Id,
                        UserBId = demoUsers[3].Id,
                        Status = MatchStatus.Pending,
                        CompatibilityScore = 64.0
                    };

                    db.Matches.AddRange(m1, m2);
                    await db.SaveChangesAsync();
                }
            }
        }
    }
}