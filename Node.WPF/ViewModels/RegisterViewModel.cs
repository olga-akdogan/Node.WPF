using Node.WPF.Helpers;
using Node.WPF.Services;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly AuthService _auth;
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

        private string _displayName = "";
        public string DisplayName
        {
            get => _displayName;
            set { if (_displayName == value) return; _displayName = value; OnPropertyChanged(); }
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
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();

                if (RegisterCommand is RelayCommand rc) rc.RaiseCanExecuteChanged();
                if (GoLoginCommand is RelayCommand lc) lc.RaiseCanExecuteChanged();
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoLoginCommand { get; }

        public RegisterViewModel(AuthService auth, MainViewModel main, NavigationService nav)
        {
            _auth = auth;
            _main = main;
            _nav = nav;

            RegisterCommand = new RelayCommand(() => _ = RegisterAsync(), () => !IsBusy);
            GoLoginCommand = new RelayCommand(() => _nav.NavigateTo<LoginViewModel>(), () => !IsBusy);
        }

        private async Task RegisterAsync()
        {
            if (IsBusy) return;

            Error = "";

            var email = (Email ?? "").Trim();
            var password = Password ?? "";
            var displayName = (DisplayName ?? "").Trim();

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(displayName))
            {
                Error = "Email, password and display name are required.";
                return;
            }

            IsBusy = true;
            try
            {
                var (ok, err) = await _auth.RegisterAsync(email, password, displayName);
                if (!ok)
                {
                    Error = err;
                    return;
                }

                _main.RefreshAuthStateFromSession();

                
                _nav.NavigateTo<BirthDataViewModel>();
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