using Node.ModelLibrary.Identity;

namespace Node.WPF.Services
{
    public class Session
    {
        public string? UserId { get; private set; }
        public string? Email { get; private set; }
        public string? DisplayName { get; private set; }

      
        public bool IsAdmin { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);

        public void SignIn(AppUser user, bool isAdmin)
        {
            UserId = user.Id;
            Email = user.Email;
            DisplayName = user.DisplayName;
            IsAdmin = isAdmin; 
        }

        public void SignOut()
        {
            UserId = null;
            Email = null;
            DisplayName = null;
            IsAdmin = false; 
        }
    }
}