using Node.WPF.Helpers;
using Node.WPF.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Node.WPF.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly NavigationService _nav;
        private readonly Session _session;
        private readonly AuthService _auth;

        public object? CurrentViewModel => _nav.CurrentViewModel;

        private bool _isAuthenticated;
        public bool IsAuthenticated
        {
            get => _isAuthenticated;
            private set
            {
                if (_isAuthenticated == value) return;
                _isAuthenticated = value;
                OnPropertyChanged();
            }
        }

        private string _userDisplayName = "";
        public string UserDisplayName
        {
            get => _userDisplayName;
            private set
            {
                if (_userDisplayName == value) return;
                _userDisplayName = value;
                OnPropertyChanged();
            }
        }

        public ICommand GoHomeCommand { get; }
        public ICommand GoLoginCommand { get; }
        public ICommand GoRegisterCommand { get; }
        public ICommand GoBirthDataCommand { get; }
        public ICommand LogoutCommand { get; }

        public MainViewModel(NavigationService nav, Session session, AuthService auth)
        {
            _nav = nav;
            _session = session;
            _auth = auth;

            _nav.PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(NavigationService.CurrentViewModel))
                    OnPropertyChanged(nameof(CurrentViewModel));
            };

            GoHomeCommand = new RelayCommand(() => _nav.NavigateTo<HomeViewModel>());
            GoLoginCommand = new RelayCommand(() => _nav.NavigateTo<LoginViewModel>());
            GoRegisterCommand = new RelayCommand(() => _nav.NavigateTo<RegisterViewModel>());
            GoBirthDataCommand = new RelayCommand(() => _nav.NavigateTo<BirthDataViewModel>());

            LogoutCommand = new RelayCommand(Logout);

            RefreshAuthStateFromSession();
        }

     
        public void Start()
        {
            RefreshAuthStateFromSession();

            if (IsAuthenticated)
                _nav.NavigateTo<HomeViewModel>();
            else
                _nav.NavigateTo<LoginViewModel>();
        }

        public void RefreshAuthStateFromSession()
        {
            IsAuthenticated = _session.IsAuthenticated;
            UserDisplayName = _session.DisplayName ?? "";
        }

        private void Logout()
        {
            _auth.Logout();
            RefreshAuthStateFromSession();
            _nav.NavigateTo<LoginViewModel>();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}