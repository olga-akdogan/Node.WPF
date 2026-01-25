using Node.WPF.Helpers;
using Node.WPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _auth;
        private readonly Session _session;
        private readonly MainViewModel _main;
        private readonly NavigationService _nav;

        private string _email = "";
        public string Email
        {
            get => _email;
            set { if (_email == value) return; _email = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { if (_password == value) return; _password = value; OnPropertyChanged(); }
        }

        private string _error = "";
        public string Error
        {
            get => _error;
            set { if (_error == value) return; _error = value; OnPropertyChanged(); }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set { if (_isBusy == value) return; _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand LoginCommand { get; }
        public ICommand GoRegisterCommand { get; }

        public LoginViewModel(AuthService auth, Session session, MainViewModel main, NavigationService nav)
        {
            _auth = auth;
            _session = session;
            _main = main;
            _nav = nav;

            LoginCommand = new RelayCommand(() => _ = LoginAsync());
            GoRegisterCommand = new RelayCommand(() => _nav.NavigateTo<RegisterViewModel>());
        }

        private async Task LoginAsync()
        {
            if (IsBusy) return;

            try
            {
                Error = "";
                IsBusy = true;

                var email = (Email ?? "").Trim();
                var password = Password ?? "";

                var (ok, error) = await _auth.LoginAsync(email, password);
                if (!ok)
                {
                    Error = error;
                    return;
                }

                _main.RefreshAuthStateFromSession();
                _nav.NavigateTo<HomeViewModel>();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}