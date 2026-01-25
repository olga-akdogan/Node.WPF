using Node.ModelLibrary.Identity;

namespace Node.WPF.Services
{
    public class Session
    {
        public string? UserId { get; private set; }
        public string? Email { get; private set; }
        public string? DisplayName { get; private set; }

        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(UserId);

        public void SignIn(AppUser user)
        {
            UserId = user.Id;
            Email = user.Email;
            DisplayName = user.DisplayName;
        }

        public void SignOut()
        {
            UserId = null;
            Email = null;
            DisplayName = null;
        }
    }
}