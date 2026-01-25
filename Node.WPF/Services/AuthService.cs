using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Node.ModelLibrary.Identity;

namespace Node.WPF.Services
{
    public class AuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly Session _session;

        public AuthService(UserManager<AppUser> userManager, Session session)
        {
            _userManager = userManager;
            _session = session;
        }

        public async Task<(bool ok, string error)> RegisterAsync(string email, string password, string displayName)
        {
            email = email.Trim().ToLowerInvariant();

            var exists = await _userManager.Users.AnyAsync(u => u.Email!.ToLower() == email);
            if (exists) return (false, "Email already exists.");

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                IsBlocked = false
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return (false, string.Join("; ", result.Errors.Select(e => e.Description)));

            
            await _userManager.AddToRoleAsync(user, "User");

            _session.SignIn(user);
            return (true, "");
        }

        public async Task<(bool ok, string error)> LoginAsync(string email, string password)
        {
            email = email.Trim().ToLowerInvariant();

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email!.ToLower() == email);
            if (user == null) return (false, "Invalid email or password.");

            if (user.IsBlocked) return (false, "Your account has been blocked. Contact support.");

            var ok = await _userManager.CheckPasswordAsync(user, password);
            if (!ok) return (false, "Invalid email or password.");

            _session.SignIn(user);
            return (true, "");
        }

        public void Logout() => _session.SignOut();
    }
}