using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;    
using Node.ModelLibrary.Data;
using Node.ModelLibrary.Identity;
using Node.ModelLibrary.Models;

namespace Node.WPF.Services
{
    class AuthService
    {
        private readonly AppDbContext _db;

        public AuthService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AppUserLocal> RegisterAsync(string email, string password)
        {
            email = email.Trim().ToLowerInvariant();

            var exists = await _db.AppUsersLocal.AnyAsync(u => u.Email == email);
            if (exists) throw new InvalidOperationException("Email already exists.");

            var user = new AppUserLocal
            {
                Email = email,
                PasswordHash = PasswordHasher.Hash(password),
                Profile = new Profile
                {
                    Bio = "",
                    Hobbies = "",
                    BirthDateTimeUtc = DateTime.SpecifyKind(DateTime.UtcNow, DateTimeKind.Utc),
                    BirthPlace = "",
                    BirthLatitude = 0,
                    BirthLongitude = 0
                }
            };

            _db.AppUsersLocal.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
